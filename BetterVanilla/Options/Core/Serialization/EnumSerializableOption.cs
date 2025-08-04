using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BetterVanilla.Core.Attributes;
using Hazel;

namespace BetterVanilla.Options.Core.Serialization;

public class EnumSerializableOption : AbstractSerializableOption
{
    private string _value;

    public string Value
    {
        get => _value;
        set
        {
            if (value == _value) return;
            _value = value;
            TriggerValueChanged();
        }
    }
    public Dictionary<string, string> AllowedValues { get; } = new();
    public int ValueIndex => Array.IndexOf(AllowedValues.Keys.ToArray(), Value);
    
    public EnumSerializableOption(
        string key,
        string title,
        object defaultValue
    ) : base(key, title)
    {
        var enumType = defaultValue.GetType();
        
        var values = Enum.GetValues(enumType);
        foreach (var value in values)
        {
            var enumKey = value.ToString()!;
            var field = enumType.GetField(enumKey)!;
            var attribute = field.GetCustomAttribute<NameAttribute>();
            AllowedValues.Add(enumKey, attribute?.Name ?? enumKey);
        }
        
        _value = defaultValue.ToString()!;
    }

    public TEnum ParseValue<TEnum>(TEnum defaultValue)
        where TEnum : struct, Enum
    {
        return Enum.TryParse<TEnum>(Value, out var value) ? value : defaultValue;
    }
    
    public override string GetValueAsString()
    {
        var value = AllowedValues.FirstOrDefault(x => x.Value.Equals(Value));
        return value.Key ?? "Unknown";
    }
    
    public override void WriteValue(MessageWriter writer)
    {
        writer.Write(Value);
    }
    
    public override void ReadValue(MessageReader reader)
    {
        Value = reader.ReadString();;
    }
    
    public override void WriteValue(BinaryWriter writer)
    {
        writer.Write(Value);
    }
    
    public override void ReadValue(BinaryReader reader)
    {
        Value = reader.ReadString();
    }
}