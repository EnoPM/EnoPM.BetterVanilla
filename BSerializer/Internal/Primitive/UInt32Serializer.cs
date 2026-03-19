using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class UInt32Serializer() : TypeSerializerBase<uint>((uint)PrimitiveTypeTag.UInt32)
{
    public override uint ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadUInt32();
    }

    public override void WriteTypedValue(BinaryWriter writer, uint value)
    {
        writer.Write(value);
    }
}
