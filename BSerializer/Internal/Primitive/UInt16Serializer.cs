using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class UInt16Serializer() : TypeSerializerBase<ushort>((uint)PrimitiveTypeTag.UInt16)
{
    public override ushort ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadUInt16();
    }

    public override void WriteTypedValue(BinaryWriter writer, ushort value)
    {
        writer.Write(value);
    }
}
