using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class Int32Serializer() : TypeSerializerBase<int>((uint)PrimitiveTypeTag.Int32)
{
    public override int ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadInt32();
    }

    public override void WriteTypedValue(BinaryWriter writer, int value)
    {
        writer.Write(value);
    }
}
