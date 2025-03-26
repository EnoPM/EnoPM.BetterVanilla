using System;
using BetterVanilla.Components.BaseComponents;
using BetterVanilla.Core;
using BetterVanilla.Core.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Components.Menu.Settings;

public sealed class NumberSettingBehaviour : BaseSettingBehaviour
{
    private const int IncrementMultiplier = 10;

    public TextMeshProUGUI valueText;
    public Button increaseButton;
    public Button decreaseButton;

    private FloatLocalOption FloatOption { get; set; }
    private IntLocalOption IntOption { get; set; }

    public float Value { get; set; }
    public float Increment { get; set; }
    public FloatRange ValidRange { get; set; }
    public string Prefix { get; set; }
    public string Suffix { get; set; }

    public override void Initialize(BaseLocalOption option)
    {
        switch (option)
        {
            case FloatLocalOption floatOption:
                FloatOption = floatOption;
                break;
            case IntLocalOption intOption:
                IntOption = intOption;
                break;
            default:
                throw new Exception($"{nameof(BaseLocalOption)} must be {nameof(FloatLocalOption)} or {nameof(IntLocalOption)}");
        }
        base.Initialize(option);
    }

    private void Awake()
    {
        increaseButton.onClick.AddListener(new Action(OnIncreaseButtonClick));
        decreaseButton.onClick.AddListener(new Action(OnDecreaseButtonClick));
    }

    protected override void Start()
    {
        if (FloatOption != null)
        {
            Increment = FloatOption.Increment;
            ValidRange = FloatOption.ValidRange;
            Prefix = FloatOption.Prefix;
            Suffix = FloatOption.Suffix;
        }
        else if (IntOption != null)
        {
            Increment = IntOption.Increment;
            ValidRange = new FloatRange(IntOption.ValidRange.min, IntOption.ValidRange.max);
            Prefix = IntOption.Prefix;
            Suffix = IntOption.Suffix;
        }
        else
        {
            throw new Exception($"{nameof(BaseLocalOption)} must be {nameof(FloatLocalOption)} or {nameof(IntLocalOption)}");
        }
        base.Start();
    }

    private void OnIncreaseButtonClick()
    {
        if (Mathf.Approximately(Value, ValidRange.max)) return;
        var multiplier = ValidRange.max - ValidRange.min >= IncrementMultiplier * Increment && LocalConditions.IsIncrementMultiplierKeyPressed() ? IncrementMultiplier : 1;
        Value = ValidRange.Clamp(Value + Increment * multiplier);
        UpdateOptionValue();
        UpdateValueText();
        AdjustButtonStates();
    }

    private void OnDecreaseButtonClick()
    {
        if (Mathf.Approximately(Value, ValidRange.min)) return;
        var multiplier = ValidRange.max - ValidRange.min >= IncrementMultiplier * Increment && LocalConditions.IsIncrementMultiplierKeyPressed() ? IncrementMultiplier : 1;
        Value = ValidRange.Clamp(Value - Increment * multiplier);
        UpdateOptionValue();
        UpdateValueText();
        AdjustButtonStates();
    }

    private void UpdateOptionValue()
    {
        if (FloatOption != null)
        {
            FloatOption.Value = Value;
        }
        else if (IntOption != null)
        {
            IntOption.Value = Mathf.RoundToInt(Value);
        }
    }

    private void UpdateValueText()
    {
        if (FloatOption != null)
        {
            valueText.SetText($"{Prefix}{Math.Round(Value, 2)}{Suffix}");
        }
        else if (IntOption != null)
        {
            valueText.SetText($"{Prefix}{Mathf.RoundToInt(Value)}{Suffix}");
        }
    }

    private void AdjustButtonStates()
    {
        increaseButton.interactable = Mathf.Approximately(Value, ValidRange.max);
        decreaseButton.interactable = Mathf.Approximately(Value, ValidRange.min);
    }

    public override void UpdateFromOption()
    {
        if (FloatOption != null)
        {
            Value = FloatOption.Value;
        }
        else if (IntOption != null)
        {
            Value = IntOption.Value;
        }
        UpdateValueText();
        AdjustButtonStates();
    }
}