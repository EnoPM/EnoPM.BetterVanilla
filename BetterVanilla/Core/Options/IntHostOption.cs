using System;
using AmongUs.GameOptions;
using BetterVanilla.Core.Extensions;
using Hazel;

namespace BetterVanilla.Core.Options;

public class IntHostOption : BaseHostOption
{
    private int _value;
    private int Value
    {
        get => _value;
        set
        {
            if (value == _value) return;
            _value = value;
            OnValueChanged();
        }
    }

    public IntHostOption(string name, string title, int defaultValue, int increment, IntRange validRange, string formatString, bool zeroIsInfinity, NumberSuffixes suffixType) : base(name, title)
    {
        var setting = InitGameSetting<IntGameSetting>(OptionTypes.Int);
        setting.Value = _value = LoadValueFromDatabase(defaultValue);
        setting.OptionName = Int32OptionNames.Invalid;
        setting.Increment = increment;
        setting.ValidRange = validRange;
        setting.FormatString = formatString;
        setting.ZeroIsInfinity = zeroIsInfinity;
        setting.SuffixType = suffixType;
    }

    public override int GetInt()
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
        Value = Behaviour.GetInt();
    }

    protected override void UpdateBehaviourValue()
    {
        if (Behaviour)
        {
            var numberOption = Behaviour.TryCast<NumberOption>();
            if (numberOption == null)
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
        Value = messageReader.ReadInt32();
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