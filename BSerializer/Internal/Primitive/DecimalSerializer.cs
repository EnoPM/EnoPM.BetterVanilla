using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class DecimalSerializer() : TypeSerializerBase<decimal>((uint)PrimitiveTypeTag.Decimal)
{
    public override decimal ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadDecimal();
    }

    public override void WriteTypedValue(BinaryWriter writer, decimal value)
    {
        writer.Write(value);
    }
}
