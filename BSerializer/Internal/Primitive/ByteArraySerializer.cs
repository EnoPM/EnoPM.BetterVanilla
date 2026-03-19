using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class ByteArraySerializer() : TypeSerializerBase<byte[]>((uint)PrimitiveTypeTag.ByteArray)
{
    public override byte[] ReadTypedValue(BinaryReader reader)
    {
        var length = reader.ReadInt32();
        return reader.ReadBytes(length);
    }

    public override void WriteTypedValue(BinaryWriter writer, byte[] value)
    {
        writer.Write(value.Length);
        writer.Write(value);
    }
}
