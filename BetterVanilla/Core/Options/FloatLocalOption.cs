using Hazel;
using UnityEngine;

namespace BetterVanilla.Core.Options;

public sealed class FloatLocalOption : BaseLocalOption
{
    private float _value;
    public float Value
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

    public readonly float Increment;
    public readonly FloatRange ValidRange;
    public readonly string Prefix;
    public readonly string Suffix;
    
    public FloatLocalOption(string name, string title, float defaultValue, float increment, FloatRange validRange, string prefix, string suffix) : base(name, title)
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
        Value = messageReader.ReadSingle();
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