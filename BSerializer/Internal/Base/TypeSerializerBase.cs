using System.IO;

namespace BSerializer.Internal.Base;

internal abstract class TypeSerializerBase(uint id)
{
    internal uint Id { get; } = id;

    public abstract object ReadValue(BinaryReader reader);
    public abstract void WriteValue(BinaryWriter writer, object value);
}

internal abstract class TypeSerializerBase<T>(uint id) : TypeSerializerBase(id)
{
    public override object ReadValue(BinaryReader reader) => ReadTypedValue(reader)!;
    public override void WriteValue(BinaryWriter writer, object value) => WriteTypedValue(writer, (T)value);

    public abstract T ReadTypedValue(BinaryReader reader);
    public abstract void WriteTypedValue(BinaryWriter writer, T value);
}
