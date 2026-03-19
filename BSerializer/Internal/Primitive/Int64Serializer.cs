using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class Int64Serializer() : TypeSerializerBase<long>((uint)PrimitiveTypeTag.Int64)
{
    public override long ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadInt64();
    }

    public override void WriteTypedValue(BinaryWriter writer, long value)
    {
        writer.Write(value);
    }
}
