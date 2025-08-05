using System;
using System.IO;
using Hazel;
using UnityEngine;

namespace BetterVanilla.Options.Core.Serialization;

public class NumberSerializableOption : AbstractSerializableOption
{
    private float _value;
    private float _minValue;
    private float _maxValue;
    private float _incrementValue;
    
    public event Action? MinValueChanged;
    public event Action? MaxValueChanged;
    public event Action? IncrementValueChanged;

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

    public float MinValue
    {
        get => _minValue;
        set
        {
            if (Mathf.Approximately(_minValue, value)) return;
            _minValue = value;
            MinValueChanged?.Invoke();
        }
    }

    public float MaxValue
    {
        get => _maxValue;
        set
        {
            if (Mathf.Approximately(_maxValue, value)) return;
            _maxValue = value;
            MaxValueChanged?.Invoke();
        }
    }

    public float IncrementValue
    {
        get => _incrementValue;
        set
        {
            if (Mathf.Approximately(_incrementValue, value)) return;
            _incrementValue = value;
            IncrementValueChanged?.Invoke();
        }
    }
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
        _incrementValue = incrementValue;
        _minValue = minValue;
        _maxValue = maxValue;
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