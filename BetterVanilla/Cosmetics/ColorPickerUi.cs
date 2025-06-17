using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Cosmetics;

public sealed class ColorPickerUi : MonoBehaviour
{
    public Color defaultColor;
    
    public ColorPickerUiController red = null!;
    public ColorPickerUiController green = null!;
    public ColorPickerUiController blue = null!;

    public TMP_InputField hexColorValueField = null!;
    public Image colorPreview = null!;
    public TextMeshProUGUI label = null!;
    
    private Coroutine? DeferredUpdateRoutine { get; set; }
    public event Action<Color>? DeferredUpdate;

    public void SetLabel(string labelText)
    {
        label.SetText(labelText);
    }

    public void SetColor(Color color)
    {
        red.SetValue(Mathf.RoundToInt(color.r * 255f));
        green.SetValue(Mathf.RoundToInt(color.g * 255f));
        blue.SetValue(Mathf.RoundToInt(color.b * 255f));
    }

    private void Awake()
    {
        red.ValueChanged += OnRGBValueChanged;
        green.ValueChanged += OnRGBValueChanged;
        blue.ValueChanged += OnRGBValueChanged;

        this.StartCoroutine(CoStart());
    }

    private IEnumerator CoStart()
    {
        yield break;
    }

    private void OnRGBValueChanged(int _)
    {
        var color = new Color(red.Value / 255f, green.Value / 255f, blue.Value / 255f, 255f);
        colorPreview.color = color;
        hexColorValueField.SetTextWithoutNotify($"#{red.Value:X2}{green.Value:X2}{blue.Value:X2}");
        
        DeferUpdate();
    }

    private void DeferUpdate()
    {
        if (DeferredUpdateRoutine != null)
        {
            StopCoroutine(DeferredUpdateRoutine);
        }
        DeferredUpdateRoutine = this.StartCoroutine(CoDeferUpdate());
    }

    private IEnumerator CoDeferUpdate()
    {
        yield return new WaitForSeconds(2f);
        DeferredUpdate?.Invoke(colorPreview.color);
        DeferredUpdateRoutine = null;
    }
}