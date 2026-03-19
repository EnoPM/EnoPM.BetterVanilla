using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class SByteSerializer() : TypeSerializerBase<sbyte>((uint)PrimitiveTypeTag.SByte)
{
    public override sbyte ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadSByte();
    }

    public override void WriteTypedValue(BinaryWriter writer, sbyte value)
    {
        writer.Write(value);
    }
}