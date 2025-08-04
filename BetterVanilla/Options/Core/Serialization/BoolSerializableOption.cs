using System.IO;
using Hazel;

namespace BetterVanilla.Options.Core.Serialization;

public class BoolSerializableOption : AbstractSerializableOption
{
    private bool _value;

    public bool Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            TriggerValueChanged();
        }
    }
    
    public BoolSerializableOption(
        string key,
        string title,
        bool defaultValue
    ) : base(key, title)
    {
        _value = defaultValue;
    }
    
    public override string GetValueAsString()
    {
        return Value ? "enabled" : "disabled";
    }
    
    public override void WriteValue(MessageWriter writer)
    {
        writer.Write(Value);
    }
    
    public override void ReadValue(MessageReader reader)
    {
        Value = reader.ReadBoolean();
    }
    
    public override void WriteValue(BinaryWriter writer)
    {
        writer.Write(Value);
    }
    
    public override void ReadValue(BinaryReader reader)
    {
        Value = reader.ReadBoolean();
    }
}