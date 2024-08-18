using System;
using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Data;
using UnityEngine;

namespace EnoPM.BetterVanilla.Core;

public sealed class FloatSetting : CustomSetting
{
    private readonly NumberRange _range;
    private readonly float _stepSize;
    private readonly string _prefix;
    private readonly string _suffix;

    public event Action<float> ValueChanged;

    public static implicit operator float(FloatSetting setting)
    {
        return setting._value;
    }

    private float _value;

    public SliderSettingItem SliderSettingBehaviour;

    public FloatSetting(string id, string title, NumberRange range, float stepSize = 1f, string prefix = "", string suffix = "", float defaultValue = default, SaveTypes saveType = SaveTypes.Local, Func<bool> isEditableFunc = null) : base(id, title, saveType, isEditableFunc)
    {
        EnsureValidRange(range, stepSize);
        _range = range;
        _stepSize = stepSize;
        _prefix = prefix;
        _suffix = suffix;
        _value = ResolveValue(defaultValue);
    }

    public void SetValue(float value)
    {
        var newValue = Mathf.Round(value / _stepSize) * _stepSize;
        if (newValue > _range.Max)
        {
            newValue = _range.Max;
        }
        else if (newValue < _range.Min)
        {
            newValue = _range.Min;
        }
        _value = newValue;
        SliderSettingBehaviour?.SetValue(newValue, true);
    }

    public override void CreateSettingUi(SettingsTabController settingsTabController)
    {
        SliderSettingBehaviour = settingsTabController.CreateSliderOption();
        SliderSettingBehaviour.SetTitle(Title);
        SliderSettingBehaviour.SetMinValue(_range.Min);
        SliderSettingBehaviour.SetMaxValue(_range.Max);
        SliderSettingBehaviour.SetStepSize(_stepSize);
        SliderSettingBehaviour.SetPrefix(_prefix);
        SliderSettingBehaviour.SetSuffix(_suffix);
        SliderSettingBehaviour.SetValue(_value);
        
        SliderSettingBehaviour.AddOnValueChangedListener(OnSettingBehaviourValueChanged);
    }

    public override void Save() => Save(_value);
    
    protected override void OnSettingBehaviourValueChanged()
    {
        _value = SliderSettingBehaviour.GetSettingValue();
        base.OnSettingBehaviourValueChanged();
        ValueChanged?.Invoke(_value);
    }

    public override SettingItem GetSettingBehaviour()
    {
        return SliderSettingBehaviour;
    }

    private void EnsureValidRange(NumberRange range, float stepSize)
    {
        var min = range.Min;
        var max = range.Max;

        if (min >= max)
        {
            throw new ArgumentException($"[{ID}] Invalid range: min value must be less than max value", nameof(range));
        }

        var delta = max - min;

        if (stepSize <= 0 || stepSize > delta)
        {
            throw new ArgumentException($"[{ID}] Invalid step size: must be greater than 0 and less than the range", nameof(stepSize));
        }
    }
}