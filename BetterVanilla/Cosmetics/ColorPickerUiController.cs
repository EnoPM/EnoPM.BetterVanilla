using System;
using BetterVanilla.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Cosmetics;

public sealed class ColorPickerUiController : MonoBehaviour
{
    public Slider slider;
    public TMP_InputField field;

    public event Action<int>? ValueChanged; 
    
    public int Value => Mathf.RoundToInt(slider.value);

    private void Awake()
    {
        field.onValueChanged.AddListener(new Action<string>(OnValueChanged));
    }

    public void OnValueChanged(float value)
    {
        Ls.LogMessage($"{gameObject.name} float value: '{(int)value}'");
        field.SetTextWithoutNotify($"{(int)value}");
        ValueChanged?.Invoke(Value);
    }

    public void SetValue(int value)
    {
        var allowedValue = Mathf.Clamp(value, 0, 255);
        if (allowedValue != Value)
        {
            slider.value = allowedValue;
            ValueChanged?.Invoke(Value);
        }
    }

    public void OnValueChanged(string value)
    {
        Ls.LogMessage($"{gameObject.name} OnValueChanged: '{value}'");
        if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out var intValue))
        {
            if (field.text != "0")
            {
                field.SetTextWithoutNotify("0");
            }
            if (slider.value != 0f)
            {
                slider.SetValueWithoutNotify(0f);
                slider.UpdateVisuals();
                ValueChanged?.Invoke(Value);
            }
            
            return;
        }
        var allowedValue = Mathf.Clamp(intValue, 0, 255);
        if (allowedValue != intValue)
        {
            field.SetTextWithoutNotify(allowedValue.ToString());
        }
        if (intValue != Value)
        {
            slider.SetValueWithoutNotify(allowedValue);
            slider.UpdateVisuals();
            ValueChanged?.Invoke(Value);
        }
    }
}