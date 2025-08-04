using System.IO;
using BetterVanilla.Core.Helpers;
using Hazel;
using UnityEngine;

namespace BetterVanilla.Options.Core.Serialization;

public class ColorSerializableOption : AbstractSerializableOption
{
    private Color _value;

    public Color Value
    {
        get => _value;
        set
        {
            if (
                Mathf.Approximately(value.r, _value.r)
                && Mathf.Approximately(value.g, _value.g)
                && Mathf.Approximately(value.b, _value.b)
                && Mathf.Approximately(value.a, _value.a)
            )
            {
                return;
            }
            _value = value;
            TriggerValueChanged();
        }
    }
    
    public ColorSerializableOption(
        string key,
        string title,
        Color defaultValue
    ) : base(key, title)
    {
        _value = defaultValue;
    }
    
    public override string GetValueAsString()
    {
        return ColorUtils.ToHex(Value);
    }
    
    public override void WriteValue(MessageWriter writer)
    {
        writer.Write(Value.r);
        writer.Write(Value.g);
        writer.Write(Value.b);
        writer.Write(Value.a);
    }
    
    public override void ReadValue(MessageReader reader)
    {
        var r = reader.ReadSingle();
        var g = reader.ReadSingle();
        var b = reader.ReadSingle();
        var a = reader.ReadSingle();
        Value = new Color(r, g, b, a);
    }
    
    public override void WriteValue(BinaryWriter writer)
    {
        writer.Write(Value.r);
        writer.Write(Value.g);
        writer.Write(Value.b);
        writer.Write(Value.a);
    }
    
    public override void ReadValue(BinaryReader reader)
    {
        var r = reader.ReadSingle();
        var g = reader.ReadSingle();
        var b = reader.ReadSingle();
        var a = reader.ReadSingle();
        Value = new Color(r, g, b, a);
    }
}