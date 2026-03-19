using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class UInt64Serializer() : TypeSerializerBase<ulong>((uint)PrimitiveTypeTag.UInt64)
{
    public override ulong ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadUInt64();
    }

    public override void WriteTypedValue(BinaryWriter writer, ulong value)
    {
        writer.Write(value);
    }
}
