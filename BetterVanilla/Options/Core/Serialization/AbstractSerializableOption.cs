using System;
using System.IO;
using Hazel;

namespace BetterVanilla.Options.Core.Serialization;

public abstract class AbstractSerializableOption
{
    public string Key { get; }
    public string Title { get; }
    
    public event Action? ValueChanged;

    protected AbstractSerializableOption(string key, string title)
    {
        Key = key;
        Title = title;
    }

    protected void TriggerValueChanged() => ValueChanged?.Invoke();
    
    public abstract string GetValueAsString();
    public abstract void WriteValue(MessageWriter writer);
    public abstract void ReadValue(MessageReader reader);
    public abstract void WriteValue(BinaryWriter writer);
    public abstract void ReadValue(BinaryReader reader);
}