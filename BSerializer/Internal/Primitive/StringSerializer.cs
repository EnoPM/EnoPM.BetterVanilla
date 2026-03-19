using System.IO;
using BSerializer.Internal.Base;

namespace BSerializer.Internal.Primitive;

internal sealed class StringSerializer() : TypeSerializerBase<string>((uint)PrimitiveTypeTag.String)
{
    public override string ReadTypedValue(BinaryReader reader)
    {
        return reader.ReadString();
    }

    public override void WriteTypedValue(BinaryWriter writer, string value)
    {
        writer.Write(value);
    }
}
