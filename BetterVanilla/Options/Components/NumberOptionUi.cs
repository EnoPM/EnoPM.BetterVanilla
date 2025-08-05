using System;
using BetterVanilla.Options.Core.Local;
using BetterVanilla.Options.Core.Serialization;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Options.Components;

public sealed class NumberOptionUi : BaseOptionUi
{
    public Slider slider = null!;
    public Button minusButton = null!;
    public Button plusButton = null!;
    public TextMeshProUGUI valueText = null!;
    
    public NumberLocalOption? SerializableOption { get; set; }

    public void SetOption(NumberLocalOption option)
    {
        SerializableOption = option;
        SerializableOption.SetUiOption(this);
        SerializableOption.RefreshUiOption();
        
        slider.onValueChanged.AddListener(new Action<float>(OnSliderValueChanged));
    }

    public void SetValueText(string text)
    {
        valueText.SetText(text);
    }

    public void SetValueWithoutNotify(float value)
    {
        slider.SetValueWithoutNotify(value);
        slider.UpdateVisuals();
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }

    public void SetMinValue(float minValue)
    {
        slider.minValue = minValue;
    }

    public void SetMaxValue(float maxValue)
    {
        slider.maxValue = maxValue;
    }

    private void OnSliderValueChanged(float value)
    {
        if (SerializableOption != null)
        {
            var stepSize = SerializableOption.IncrementValue;
            var snapped = Mathf.Round(value / stepSize) * stepSize;
            if (!Mathf.Approximately(snapped, value))
            {
                slider.SetValueWithoutNotify(snapped);
                slider.UpdateVisuals();
            }
            valueText.SetText($"{SerializableOption.ValuePrefix}{slider.value}{SerializableOption.ValueSuffix}");
            SerializableOption.Value = snapped;
        }
        else
        {
            valueText.SetText($"{slider.value:F2}");
        }
    }

    public void OnMinusButtonClicked()
    {
        var stepSize = SerializableOption?.IncrementValue ?? slider.stepSize;
        var value = slider.value - stepSize;
        if (value < slider.minValue)
        {
            value = slider.minValue;
        }
        else if (slider.value > slider.maxValue)
        {
            value = slider.maxValue;
        }
        slider.value = value;
    }

    public void OnPlusButtonClicked()
    {
        var stepSize = SerializableOption?.IncrementValue ?? slider.stepSize;
        var value = slider.value + stepSize;
        if (value < slider.minValue)
        {
            value = slider.minValue;
        }
        else if (slider.value > slider.maxValue)
        {
            value = slider.maxValue;
        }
        slider.value = value;
    }
    
    private void Update()
    {
        SerializableOption?.RefreshUiLock();
    }

    public override void RefreshVisibility()
    {
        SerializableOption?.RefreshUiVisibility();
    }
}