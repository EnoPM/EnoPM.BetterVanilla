using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BSerializer.Internal;

internal sealed class InternalDeserializer
{
    private readonly List<SerializedTypeDefinition> _definitions = [];

    public T Deserialize<T>(byte[] data) where T : new()
    {
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream);

        ReadSchema(reader);
        return (T)ReadValue(reader, typeof(T));
    }

    private void ReadSchema(BinaryReader reader)
    {
        var definitionCount = reader.ReadUInt32();
        for (uint i = 0; i < definitionCount; i++)
        {
            var id = reader.ReadUInt32();
            var name = reader.ReadString();
            var propertyCount = reader.ReadUInt32();

            var definition = new SerializedTypeDefinition(id, name);
            for (uint j = 0; j < propertyCount; j++)
            {
                var propId = reader.ReadUInt32();
                var propTypeId = reader.ReadUInt32();
                var propName = reader.ReadString();
                definition.Properties.Add(new SerializedPropertyDefinition(propId, propTypeId, propName));
            }

            _definitions.Add(definition);
        }
    }

    private object ReadValue(BinaryReader reader, Type type)
    {
        if (Primitives.Serializers.TryGetValue(type, out var serializer))
        {
            return serializer.ReadValue(reader);
        }

        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var count = reader.ReadInt32();
            var array = Array.CreateInstance(elementType, count);
            for (var i = 0; i < count; i++)
            {
                array.SetValue(ReadElement(reader, elementType), i);
            }
            return array;
        }

        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();
            if (genericDef == typeof(List<>))
            {
                var elementType = type.GetGenericArguments()[0];
                var count = reader.ReadInt32();
                var list = (IList)Activator.CreateInstance(type)!;
                for (var i = 0; i < count; i++)
                {
                    list.Add(ReadElement(reader, elementType));
                }
                return list;
            }
            if (genericDef == typeof(Dictionary<,>))
            {
                var args = type.GetGenericArguments();
                var count = reader.ReadInt32();
                var dict = (IDictionary)Activator.CreateInstance(type)!;
                for (var i = 0; i < count; i++)
                {
                    var key = ReadElement(reader, args[0])!;
                    var value = ReadElement(reader, args[1]);
                    dict.Add(key, value);
                }
                return dict;
            }
        }

        var definition = _definitions.First(x => x.Name == type.FullName);
        var obj = Activator.CreateInstance(type)!;
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var propDef in definition.Properties)
        {
            var property = properties.First(p => p.Name == propDef.Name);
            var value = ReadElement(reader, property.PropertyType);
            if (value != null)
            {
                property.SetValue(obj, value);
            }
        }

        return obj;
    }

    private object ReadElement(BinaryReader reader, Type type)
    {
        if (type.IsValueType)
        {
            return ReadValue(reader, type);
        }

        var isNotNull = reader.ReadBoolean();
        return isNotNull ? ReadValue(reader, type) : null;
    }
}