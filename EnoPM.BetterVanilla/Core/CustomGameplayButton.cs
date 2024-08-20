using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using EnoPM.BetterVanilla.Core.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Core;

public abstract class CustomGameplayButton : MonoBehaviour
{
    public CustomGameplayButtonData Data { get; private set; }
    public GameObject GameObject { get; private set; }
    public SpriteRenderer Graphic { get; private set; }
    public GameObject CommunicationsDown { get; private set; }
    public ActionMapGlyphDisplay Glyph { get; private set; }
    
    protected TextMeshPro UsesRemainingText { get; private set; }
    protected SpriteRenderer UsesRemainingGraphic { get; private set; }
    protected TextMeshPro TimerText { get; private set; }
    protected TextMeshPro LabelText { get; private set; }
    
    private int InternalUsesRemaining { get; set; }
    private Vector3? SavedGraphicPosition { get; set; }
    private Vector3? SavedUsesRemainingPosition { get; set; }

    protected Coroutine Effect { get; set; }
    protected float CurrentCooldown { get; set; }
    protected float MaxCooldown { get; set; }

    public bool IsTimerActive => TimerText.gameObject.activeSelf;
    public bool IsLabelActive => LabelText.gameObject.activeSelf;
    public bool IsEffectActive => Effect != null && !Effect.WasCollected;
    public string Name => GameObject.name;
    public bool IsUsesRemainingActive => UsesRemainingText.gameObject.activeSelf && UsesRemainingGraphic.gameObject.activeSelf;
    
    protected bool IsShaking => SavedGraphicPosition.HasValue && SavedUsesRemainingPosition.HasValue;
    protected int UsesRemaining
    {
        get => InternalUsesRemaining;
        set
        {
            if (InternalUsesRemaining == value) return;
            InternalUsesRemaining = value;
            var text = InternalUsesRemaining.ToString();
            if (text == UsesRemainingText.text) return;
            UsesRemainingText.SetText(text);
        }
    }
    
    public void SetActive(bool value)
    {
        if (GameObject.activeSelf == value) return;
        GameObject.SetActive(value);
    }
    
    public void SetUsesRemainingActive(bool value)
    {
        if (UsesRemainingText.gameObject.activeSelf != value)
        {
            UsesRemainingText.gameObject.SetActive(value);
        }
        if (UsesRemainingGraphic.gameObject.activeSelf != value)
        {
            UsesRemainingGraphic.gameObject.SetActive(value);
        }
    }
    
    public void SetUsesRemainingEnabled(bool value)
    {
        var color = value ? Palette.EnabledColor : Palette.DisabledGrey;
        if (UsesRemainingGraphic.color == color) return;
        UsesRemainingGraphic.color = color;
    }
    
    public void SetTimerActive(bool value)
    {
        if (IsTimerActive == value) return;
        TimerText.gameObject.SetActive(value);
    }

    public void SetTimerText(string value)
    {
        if (TimerText.text == value) return;
        TimerText.SetText(value);
    }
    
    public void SetFillPercentage(float value)
    {
        if (Graphic.material.GetFloat(ShaderProperties.Percent).Approx(value)) return;
        Graphic.material.SetFloat(ShaderProperties.Percent, value);
    }
    
    public void SetTimer(float timer, float maxTimer, bool invertedFill = false)
    {
        var percentage = maxTimer == 0f ? 0f : Mathf.Clamp(timer / maxTimer, 0f, 1f);
        SetTimerActive(percentage > 0f);
        SetTimerText(Mathf.CeilToInt(timer).ToString());
        SetFillPercentage(invertedFill ? 1f - percentage : percentage);
    }
    
    public void SetEffectTimer(float timer, float maxTimer) => SetTimer(timer, maxTimer, true);

    public void SetLabelActive(bool value)
    {
        if (LabelText.gameObject.activeSelf == value) return;
        LabelText.gameObject.SetActive(value);
    }
    
    public void SetLabelFaceColor(Color value)
    {
        if (LabelText.faceColor == value) return;
        LabelText.faceColor = value;
    }

    public void SetLabelOutlineColor(Color value)
    {
        if (LabelText.outlineColor == value) return;
        LabelText.outlineColor = value;
    }

    public void SetLabelUnderlayColor(Color value)
    {
        if (LabelText.fontMaterial.GetColor(ShaderProperties.UnderlayColor) == value) return;
        LabelText.fontMaterial.SetColor(ShaderProperties.UnderlayColor, value);
    }

    public void SetLabelEnabled(bool value)
    {
        var color = value ? Palette.EnabledColor : Palette.DisabledClear;
        if (LabelText.color == color) return;
        LabelText.color = color;
    }
    
    protected void StartShaking()
    {
        if (IsShaking)
        {
            throw new Exception("Shaking already started");
        }
        SavedGraphicPosition = Graphic.transform.localPosition;
        SavedUsesRemainingPosition = UsesRemainingGraphic.transform.localPosition;
    }

    protected void Shake(float strength = 0.01f)
    {
        if (!IsShaking)
        {
            throw new Exception("Shaking must be started");
        }
        var randomOffset = (Vector3) UnityEngine.Random.insideUnitCircle * strength;
        if (SavedGraphicPosition != null)
        {
            Graphic.transform.localPosition = SavedGraphicPosition.Value + randomOffset ;
        }
        if (SavedUsesRemainingPosition != null)
        {
            UsesRemainingGraphic.transform.localPosition = SavedUsesRemainingPosition.Value + randomOffset;
        }
    }

    protected void EndShaking()
    {
        if (!IsShaking)
        {
            throw new Exception("Unable to end shaking: not started");
        }
        if (SavedGraphicPosition.HasValue)
        {
            Graphic.transform.localPosition = SavedGraphicPosition.Value;
            SavedGraphicPosition = null;
        }
        if (SavedUsesRemainingPosition.HasValue)
        {
            UsesRemainingGraphic.transform.localPosition = SavedUsesRemainingPosition.Value;
            SavedUsesRemainingPosition = null;
        }
    }
    
    protected virtual void Awake()
    {
        var type = GetType();
        
        var abilityButton = Instantiate(HudManager.Instance.AbilityButton, gameObject.transform);
        abilityButton.enabled = false;
        
        GameObject = abilityButton.gameObject;
        GameObject.name = type.Name;
        
        Graphic = abilityButton.graphic;

        var passive = abilityButton.GetComponent<PassiveButton>();
        passive.OnClick = new Button.ButtonClickedEvent();
        passive.OnClick.AddListener((UnityAction)DoClick);

        UsesRemainingText = abilityButton.usesRemainingText;
        UsesRemainingGraphic = abilityButton.usesRemainingSprite;
        SetUsesRemainingActive(false);

        TimerText = abilityButton.cooldownTimerText;

        LabelText = abilityButton.buttonLabelText;
        
        LabelText.SetText(string.Empty);
        SetLabelActive(true);
        SetLabelEnabled(true);
        
        Glyph = abilityButton.glyph;
        Glyph.gameObject.SetActive(false);
        Glyph.SetSpriteVisibility(false);
        
        CommunicationsDown = abilityButton.commsDown;
        CommunicationsDown.SetActive(false);

        Data = new CustomGameplayButtonData(type);
    }

    public void UpdateParent(Transform parent)
    {
        GameObject.transform.SetParent(parent);
    }
    
    private void OnDestroy()
    {
        Destroy(GameObject);
    }
    
    protected virtual void Update()
    {
        if (!IsVisible())
        {
            GameObject.SetActive(false);
            return;
        }
        if (Data.HasEffect && IsEffectActive)
        {
            SetEnabled(true);
        }
        else
        {
            if (IsUsesRemainingActive && UsesRemaining <= 0)
            {
                SetEnabled(false);
                SetTimer(0f, MaxCooldown);
            }
            else
            {
                if (CurrentCooldown > 0f && IsCoolingDown())
                {
                    CurrentCooldown -= Time.deltaTime;
                    SetTimer(CurrentCooldown, MaxCooldown);
                }
                SetEnabled(CanClick());
            }
        }
    }
    
    public void DoClick()
    {
        if (CurrentCooldown > 0f) return;
        if (IsUsesRemainingActive && UsesRemaining <= 0) return;
        if (IsEffectActive) return;
        if (!CanClick()) return;
        if (IsUsesRemainingActive)
        {
            UsesRemaining--;
        }
        if (Data.HasEffect)
        {
            StartEffect();
        }
        else
        {
            OnClicked();
        }
    }
    
    public void StartEffect()
    {
        if (IsEffectActive)
        {
            Plugin.Logger.LogWarning($"Trying to set an existing effect in {Name}");
            return;
        }
        Effect = this.StartCoroutine(CoRunEffect());
    }

    public void StopEffect()
    {
        if (IsEffectActive)
        {
            StopCoroutine(Effect);
        }
        
        Effect = null;
    }
    
    public void SetSprite(Sprite sprite)
    {
        if (Graphic.sprite == sprite) return;
        Graphic.sprite = sprite;
        Graphic.SetCooldownNormalizedUvs();
    }

    public void SetEnabled(bool value)
    {
        SetLabelEnabled(value);
        SetUsesRemainingEnabled(value);
        var rendererColor = value ? Palette.EnabledColor : Palette.DisabledClear;
        if (Graphic.color != rendererColor)
        {
            Graphic.color = rendererColor;
        }
        var desat = value ? 0f : 1f;
        if (!Graphic.material.GetFloat(ShaderProperties.Desat).Approx(desat))
        {
            Graphic.material.SetFloat(ShaderProperties.Desat, desat);
        }
    }
    
    private IEnumerator CoRunEffect()
    {
        yield return CoEffect();
        StopEffect();
        OnEffectEnded();
    }
    
    protected virtual void OnEffectEnded()
    {
        CurrentCooldown = MaxCooldown;
        SetTimer(CurrentCooldown, MaxCooldown);
    }

    public virtual void OnClicked()
    {
        CurrentCooldown = MaxCooldown;
        SetTimer(CurrentCooldown, MaxCooldown);
    }
    
    protected virtual bool CanClick() => true;
    protected virtual bool IsVisible() => true;
    
    protected virtual bool IsCoolingDown() => true;
    
    protected virtual IEnumerator CoEffect()
    {
        yield break;
    }
}