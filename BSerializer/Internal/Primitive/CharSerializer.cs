using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class CharSerializer() : TypeSerializerBase<char>((uint)PrimitiveTypeTag.Char)
{
    public override char ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadChar();
    }

    public override void WriteTypedValue(BinaryWriter writer, char value)
    {
        writer.Write(value);
    }
}
