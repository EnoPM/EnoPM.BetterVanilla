using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class DoubleSerializer() : TypeSerializerBase<double>((uint)PrimitiveTypeTag.Double)
{
    public override double ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadDouble();
    }

    public override void WriteTypedValue(BinaryWriter writer, double value)
    {
        writer.Write(value);
    }
}
