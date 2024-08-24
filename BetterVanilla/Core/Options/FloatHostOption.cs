using System;
using AmongUs.GameOptions;
using BetterVanilla.Core.Extensions;
using Hazel;
using UnityEngine;

namespace BetterVanilla.Core.Options;

public class FloatHostOption : BaseHostOption
{
    private float _value;
    private float Value
    {
        get => _value;
        set
        {
            if (Mathf.Approximately(value, _value)) return;
            _value = value;
            OnValueChanged();
        }
    }

    private readonly FloatGameSetting _settings;

    public FloatHostOption(string name, string title, float defaultValue, float increment, FloatRange validRange, string formatString, bool zeroIsInfinity, NumberSuffixes suffixType) : base(name, title)
    {
        _settings = InitGameSetting<FloatGameSetting>(OptionTypes.Float);
        _settings.Value = _value = LoadValueFromDatabase(defaultValue);
        _settings.OptionName = FloatOptionNames.Invalid;
        _settings.Increment = increment;
        _settings.ValidRange = validRange;
        _settings.FormatString = formatString;
        _settings.ZeroIsInfinity = zeroIsInfinity;
        _settings.SuffixType = suffixType;
    }

    public override float GetFloat()
    {
        return Value;
    }

    public override void OnBehaviourCreated(OptionBehaviour behaviour)
    {
        base.OnBehaviourCreated(behaviour);
        if (behaviour is not NumberOption numberOption)
        {
            throw new ArgumentException($"behaviour must be {nameof(NumberOption)}", nameof(behaviour));
        }
        numberOption.TitleText.SetText(Title);
        numberOption.Value = Value;
        numberOption.AdjustButtonsActiveState();
    }

    public override void UpdateValueFromBehaviour()
    {
        Value = Behaviour.GetFloat();
    }

    protected override void UpdateBehaviourValue()
    {
        if (Behaviour)
        {
            var numberOption = Behaviour.TryCast<NumberOption>();
            if (!numberOption)
            {
                throw new ArgumentException($"behaviour must be {nameof(NumberOption)}");
            }
            numberOption.Value = Value;
            numberOption.AdjustButtonsActiveState();
        }
        if (ViewBehaviour)
        {
            ViewBehaviour?.CustomSetInfo(61, this);
        }
    }

    public override void WriteValue(MessageWriter messageWriter)
    {
        messageWriter.Write(Value);
    }

    public override void ReadValue(MessageReader messageReader)
    {
        Value = messageReader.ReadSingle();
    }
    public override string GetValueString()
    {
        return GameSetting.GetValueString(Value);
    }

    protected override void OnValueChanged()
    {
        base.OnValueChanged();
        SaveValueInDatabase(Value);
    }
}