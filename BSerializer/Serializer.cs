using System.Diagnostics.CodeAnalysis;
using System.IO;
using BSerializer.Internal;

namespace BSerializer;

public static class Serializer
{
    // Reflection-based API (existing, for non-AOT scenarios)
    [RequiresUnreferencedCode("Use override with BSerializerContext for AOT support")]
    public static byte[] Serialize<T>(T obj) where T : new()
    {
        var serializer = new InternalSerializer(typeof(T));
        return serializer.Serialize(obj);
    }

    [RequiresUnreferencedCode("Use override with BSerializerContext for AOT support")]
    public static void Serialize<T>(T obj, Stream stream) where T : new()
    {
        var serialized = Serialize(obj);
        stream.Write(serialized, 0, serialized.Length);
    }

    [RequiresUnreferencedCode("Use override with BSerializerContext for AOT support")]
    public static T Deserialize<T>(byte[] data) where T : new()
    {
        var deserializer = new InternalDeserializer();
        return deserializer.Deserialize<T>(data);
    }

    [RequiresUnreferencedCode("Use override with BSerializerContext for AOT support")]
    public static T Deserialize<T>(Stream stream) where T : new()
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return Deserialize<T>(ms.ToArray());
    }

    // AOT-safe API (uses source-generated context)
    public static byte[] Serialize<T>(T obj, BSerializerContext context) where T : new()
        => context.Serialize(obj);

    public static T Deserialize<T>(byte[] data, BSerializerContext context) where T : new()
        => context.Deserialize<T>(data);
}
