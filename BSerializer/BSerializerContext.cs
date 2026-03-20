using System;

namespace BSerializer;

public abstract class BSerializerContext
{
    public byte[] Serialize<T>(T obj) where T : new()
        => SerializeCore(obj!, typeof(T));

    public T Deserialize<T>(byte[] data) where T : new()
        => (T)DeserializeCore(data, typeof(T));

    protected abstract byte[] SerializeCore(object obj, Type type);
    protected abstract object DeserializeCore(byte[] data, Type type);
}
