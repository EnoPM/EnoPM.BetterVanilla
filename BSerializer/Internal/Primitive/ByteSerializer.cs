using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class ByteSerializer() : TypeSerializerBase<byte>((uint)PrimitiveTypeTag.Byte)
{
    public override byte ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadByte();
    }

    public override void WriteTypedValue(BinaryWriter writer, byte value)
    {
        writer.Write(value);
    }
}