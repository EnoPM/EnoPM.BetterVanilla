using System.IO;
using Hazel;

namespace BetterVanilla.Options.Core.Serialization;

public class TextSerializableOption : AbstractSerializableOption
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
    
    public int MaxLength { get; }
    
    public TextSerializableOption(
        string key,
        string title,
        string defaultValue,
        int maxLength
    ) : base(key, title)
    {
        _value = defaultValue;
        MaxLength = maxLength;
    }
    
    public override string GetValueAsString()
    {
        return Value;
    }
    
    public override void WriteValue(MessageWriter writer)
    {
        writer.Write(Value);
    }
    
    public override void ReadValue(MessageReader reader)
    {
        Value = reader.ReadString();
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