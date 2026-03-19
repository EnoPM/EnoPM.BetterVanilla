using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class Int16Serializer() : TypeSerializerBase<short>((uint)PrimitiveTypeTag.Int16)
{
    public override short ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadInt16();
    }

    public override void WriteTypedValue(BinaryWriter writer, short value)
    {
        writer.Write(value);
    }
}