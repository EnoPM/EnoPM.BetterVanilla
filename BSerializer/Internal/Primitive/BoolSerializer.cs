using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class BoolSerializer() : TypeSerializerBase<bool>((uint)PrimitiveTypeTag.Bool)
{
    public override bool ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadBoolean();
    }

    public override void WriteTypedValue(BinaryWriter writer, bool value)
    {
        writer.Write(value);
    }
}