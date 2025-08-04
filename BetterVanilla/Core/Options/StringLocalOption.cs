using System;
using System.Collections.Generic;
using Hazel;

namespace BetterVanilla.Core.Options;

public class StringLocalOption : BaseLocalOption
{
    private int _index;
    private readonly List<string> _realValues;
    public readonly List<string> Values;

    public int Index
    {
        get => _index;
        set
        {
            if (value == _index || value < 0 || value >= Values.Count) return;
            _index = value;
            OnValueChanged();
        }
    }
    
    public string Value => Values[_index];
    public string RealValue => _realValues[_index];

    private int FindIndex(string value)
    {
        var index = Values.IndexOf(value);
        if (index < 0 || index >= Values.Count)
        {
            Ls.LogWarning($"Unable to find '{value}' in Values in {nameof(StringLocalOption)} named {Name}");
            index = 0;
        }
        return index;
    }
    
    public StringLocalOption(string name, string title, string defaultValue, List<string> values, List<string> realValues) : base(name, title)
    {
        _realValues = realValues;
        Values = values;
        _index = LoadValueFromDatabase(FindIndex(defaultValue));
    }
    
    public override void WriteValue(MessageWriter messageWriter)
    {
        messageWriter.Write(Index);
    }

    public override void ReadValue(MessageReader messageReader)
    {
        Index = messageReader.ReadInt32();
    }
    
    public override string GetValueString()
    {
        return Value;
    }

    public TEnum ParseValue<TEnum>(TEnum defaultValue) where TEnum : struct, Enum
    {
        return Enum.TryParse<TEnum>(RealValue, out var value) ? value : defaultValue;
    }

    protected override void OnValueChanged()
    {
        base.OnValueChanged();
        SaveValueInDatabase(Index);
    }
}