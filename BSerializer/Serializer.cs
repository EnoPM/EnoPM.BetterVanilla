using System.IO;
using BSerializer.Internal;

namespace BSerializer;

public static class Serializer
{
    public static byte[] Serialize<T>(T obj) where T : new()
    {
        var serializer = new InternalSerializer(typeof(T));
        return serializer.Serialize(obj);
    }

    public static void Serialize<T>(T obj, Stream stream) where T : new()
    {
        var serialized = Serialize(obj);
        stream.Write(serialized, 0, serialized.Length);
    }

    public static T Deserialize<T>(byte[] data) where T : new()
    {
        var deserializer = new InternalDeserializer();
        return deserializer.Deserialize<T>(data);
    }

    public static T Deserialize<T>(Stream stream) where T : new()
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return Deserialize<T>(ms.ToArray());
    }
}