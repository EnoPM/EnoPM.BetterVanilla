using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class FloatSerializer() : TypeSerializerBase<float>((uint)PrimitiveTypeTag.Float)
{
    public override float ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadSingle();
    }

    public override void WriteTypedValue(BinaryWriter writer, float value)
    {
        writer.Write(value);
    }
}
