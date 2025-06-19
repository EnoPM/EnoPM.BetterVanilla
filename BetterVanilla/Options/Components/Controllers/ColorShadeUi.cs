using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Options.Components.Controllers;

public sealed class ColorShadeUi : MonoBehaviour
{
    public Slider slider = null!;
    public TMP_InputField field = null!;
    
    public event Action? ShadeUpdated;
    public int Value => Mathf.RoundToInt(slider.value);

    public void SetValueWithoutNotify(float value)
    {
        if (value is < 0f or > 255f)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be between 0 and 255.");
        }
        
        slider.SetValueWithoutNotify(value);
        field.SetTextWithoutNotify($"{Mathf.RoundToInt(value)}");
    }

    private void Awake()
    {
        slider.onValueChanged
            .AddListener(new Action<float>(OnSliderValueChanged));
        field.onEndEdit
            .AddListener(new Action<string>(OnFieldValueChanged));
    }

    private void OnFieldValueChanged(string value)
    {
        if (!int.TryParse(value, out var intValue) || intValue < 0)
        {
            field.SetText("0");
            return;
        }
        if (intValue > 255)
        {
            field.SetText("255");
            return;
        }
        slider.SetValueWithoutNotify(intValue);
        slider.UpdateVisuals();
        ShadeUpdated?.Invoke();
    }

    private void OnSliderValueChanged(float value)
    {
        field.SetTextWithoutNotify($"{Mathf.RoundToInt(value)}");
        ShadeUpdated?.Invoke();
    }
}