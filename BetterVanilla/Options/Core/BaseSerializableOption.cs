using System.IO;
using Hazel;

namespace BetterVanilla.Options.Core;

public abstract class BaseSerializableOption
{
    public string Name { get; set; }
    public string Title { get; }

    public abstract string DisplayValue { get; }

    protected BaseSerializableOption(string name, string? title = null)
    {
        Name = name;
        Title = title ?? Name;
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Name);
        SerializeValue(writer);
    }
    public void Deserialize(BinaryReader reader) => DeserializeValue(reader);
    protected abstract void SerializeValue(BinaryWriter writer);
    protected abstract void DeserializeValue(BinaryReader reader);

    public void Serialize(MessageWriter writer)
    {
        writer.Write(Name);
        SerializeValue(writer);
    }

    public void Deserialize(MessageReader reader) => DeserializeValue(reader);

    protected abstract void SerializeValue(MessageWriter writer);

    protected abstract void DeserializeValue(MessageReader reader);
}