using Hazel;
using UnityEngine;

namespace BetterVanilla.Core.Options;

public sealed class IntLocalOption : BaseLocalOption
{
    private int _value;
    public int Value
    {
        get => _value;
        set
        {
            var valueToSet = value;
            if (valueToSet > ValidRange.max)
            {
                valueToSet = ValidRange.max;
            }
            else if (valueToSet < ValidRange.min)
            {
                valueToSet = ValidRange.min;
            }
            if (Mathf.Approximately(valueToSet, _value)) return;
            _value = valueToSet;
            OnValueChanged();
        }
    }

    public readonly int Increment;
    public readonly IntRange ValidRange;
    public readonly string Prefix;
    public readonly string Suffix;
    
    public IntLocalOption(string name, string title, int defaultValue, int increment, IntRange validRange, string prefix, string suffix) : base(name, title)
    {
        Increment = increment;
        ValidRange = validRange;
        Prefix = prefix;
        Suffix = suffix;
        _value = LoadValueFromDatabase(defaultValue);
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
        return $"{Prefix}{Value}{Suffix}";
    }

    protected override void OnValueChanged()
    {
        base.OnValueChanged();
        SaveValueInDatabase(Value);
    }
}