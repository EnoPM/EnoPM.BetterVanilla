using AmongUs.GameOptions;
using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Options.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Options.Core.Host;

public sealed class NumberHostOption : NumberSerializableOption, IHostOption<FloatGameSetting, NumberOption>
{
    public FloatGameSetting GameSetting { get; }
    public NumberOption? SettingBehaviour { get; private set; }
    public ViewSettingsInfoPanel? ViewSettingBehaviour { get; private set; }

    public NumberHostOption(string key, string title, float defaultValue, float incrementValue, float minValue, float maxValue, string valuePrefix = "", string valueSuffix = "") : base(key, title, defaultValue, incrementValue, minValue, maxValue, valuePrefix, valueSuffix)
    {
        GameSetting = ScriptableObject.CreateInstance<FloatGameSetting>();
        GameSetting.Type = OptionTypes.Float;
        GameSetting.hideFlags = HideFlags.DontSave;
        GameSetting.name = $"BetterVanillaCustomSetting_{key}";
        GameSetting.Title = StringNames.None;
        GameSetting.Value = Value;
        GameSetting.OptionName = FloatOptionNames.Invalid;
        GameSetting.Increment = IncrementValue;
        GameSetting.FormatString = "0.00";
        GameSetting.ZeroIsInfinity = false;
        GameSetting.SuffixType = NumberSuffixes.Seconds;
        GameSetting.ValidRange = new FloatRange(MinValue, MaxValue);
        ValueChanged += OnValueChanged;
    }

    private void OnValueChanged()
    {
        if (LocalConditions.AmHost() && BetterPlayerControl.LocalPlayer != null)
        {
            BetterPlayerControl.LocalPlayer.RpcSetHostOptionValue(this);
        }
        UpdateBehaviours();
    }
    
    public BaseGameSetting GetGameSetting() => GameSetting;
    public OptionBehaviour? GetOptionBehaviour() => SettingBehaviour;
    
    public void OnBehaviourCreated(NumberOption behaviour)
    {
        SettingBehaviour = behaviour;
        SettingBehaviour.TitleText.SetText(Title);
        SettingBehaviour.TitleText.color = Color.magenta;
        SettingBehaviour.Value = Value;
        SettingBehaviour.AdjustButtonsActiveState();
    }
    
    public void OnViewBehaviourCreated(ViewSettingsInfoPanel viewBehaviour)
    {
        ViewSettingBehaviour = viewBehaviour;
    }
    
    public void UpdateValueFromSettingBehaviour()
    {
        if (SettingBehaviour == null) return;
        Value = SettingBehaviour.GetFloat();
    }
    
    public void UpdateBehaviours()
    {
        if (SettingBehaviour != null)
        {
            SettingBehaviour.Value = Value;
            SettingBehaviour.AdjustButtonsActiveState();
        }
        if (ViewSettingBehaviour != null)
        {
            SetViewSettingBehaviourInfo(ViewSettingBehaviour, 61);
        }
    }
    
    public void SetViewSettingBehaviourInfo(ViewSettingsInfoPanel viewBehaviour, int maskLayer)
    {
        var allowed = IsAllowed();
        var color = allowed ? Palette.EnabledColor : Palette.DisabledClear;
        viewBehaviour.titleText.SetText(Title);
        viewBehaviour.titleText.color = Color.magenta;
        viewBehaviour.settingText.SetText(GetValueAsString());
        viewBehaviour.disabledBackground.gameObject.SetActive(false);
        viewBehaviour.background.gameObject.SetActive(true);
        viewBehaviour.SetMaskLayer(maskLayer);
        if (viewBehaviour.settingText.color != color)
        {
            viewBehaviour.settingText.color = color;
        }
    }
    
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        if (SettingBehaviour == null) return;
        var interactable = IsAllowed();
        var color = interactable ? Palette.EnabledColor : Palette.DisabledClear;
        SettingBehaviour.MinusBtn.SetInteractable(interactable);
        SettingBehaviour.PlusBtn.SetInteractable(interactable);
        SettingBehaviour.MinusBtn.enabled = interactable;
        SettingBehaviour.PlusBtn.enabled = interactable;
        if (SettingBehaviour.ValueText.color != color)
        {
            SettingBehaviour.ValueText.color = color;
        }
        if (interactable)
        {
            SettingBehaviour.AdjustButtonsActiveState();
        }
    }
}