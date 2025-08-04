using System;
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
    
    public NumberSerializableOption? SerializableOption { get; set; }

    public void SetOption(NumberSerializableOption option)
    {
        SerializableOption = option;
        SetLabel(option.Title);
        RefreshOptionStates();
        slider.value = SerializableOption.Value;
        valueText.SetText(SerializableOption.GetValueAsString());
        
        slider.onValueChanged.AddListener(new Action<float>(OnSliderValueChanged));
    }

    public void RefreshOptionStates()
    {
        if (SerializableOption == null) return;
        slider.minValue = SerializableOption.MinValue;
        slider.maxValue = SerializableOption.MaxValue;
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
}