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
    
    public TextSerializableOption(
        string key,
        string title,
        string defaultValue
    ) : base(key, title)
    {
        _value = defaultValue;
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