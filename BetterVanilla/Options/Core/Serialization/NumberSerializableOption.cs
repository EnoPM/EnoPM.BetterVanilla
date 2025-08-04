using System.IO;
using Hazel;
using UnityEngine;

namespace BetterVanilla.Options.Core.Serialization;

public class NumberSerializableOption : AbstractSerializableOption
{
    private float _value;

    public float Value
    {
        get => _value;
        set
        {
            if (Mathf.Approximately(_value, value)) return;
            _value = value;
            TriggerValueChanged();
        }
    }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    public float IncrementValue { get; }
    public string ValuePrefix { get; set; }
    public string ValueSuffix { get; set; }

    public NumberSerializableOption(
        string key,
        string title,
        float defaultValue,
        float incrementValue,
        float minValue,
        float maxValue,
        string valuePrefix = "",
        string valueSuffix = ""
    ) : base(key, title)
    {
        _value = defaultValue;
        IncrementValue = incrementValue;
        MinValue = minValue;
        MaxValue = maxValue;
        ValuePrefix = valuePrefix;
        ValueSuffix = valueSuffix;
    }

    public override string GetValueAsString()
    {
        var formattedValue = Value % 1 == 0 ? $"{Value:F0}" : $"{Value:F2}";
        return $"{ValuePrefix}{formattedValue}{ValueSuffix}";
    }

    public override void WriteValue(MessageWriter writer)
    {
        writer.Write(Value);
    }

    public override void ReadValue(MessageReader reader)
    {
        Value = reader.ReadSingle();
    }

    public override void WriteValue(BinaryWriter writer)
    {
        writer.Write(Value);
    }

    public override void ReadValue(BinaryReader reader)
    {
        Value = reader.ReadSingle();
    }
}