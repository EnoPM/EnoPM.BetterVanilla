using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BSerializer.Internal;

internal sealed class InternalSerializer
{
    private SerializedDocument Document { get; } = new();

    public InternalSerializer(Type type)
    {
        NextId = Primitives.Serializers.Values.Max(x => x.Id) + 1;
        GetTypeId(type);
    }

    private uint NextId => field++;

    public byte[] Serialize(object obj)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        WriteSchema(writer);
        WriteValue(writer, obj, obj.GetType());

        return stream.ToArray();
    }

    private void WriteSchema(BinaryWriter writer)
    {
        writer.Write((uint)Document.Definitions.Count);
        foreach (var definition in Document.Definitions)
        {
            writer.Write(definition.Id);
            writer.Write(definition.Name);
            writer.Write((uint)definition.Properties.Count);
            foreach (var property in definition.Properties)
            {
                writer.Write(property.Id);
                writer.Write(property.TypeId);
                writer.Write(property.Name);
            }
        }
    }

    private void WriteValue(BinaryWriter writer, object obj, Type type)
    {
        if (Primitives.Serializers.TryGetValue(type, out var serializer))
        {
            serializer.WriteValue(writer, obj);
            return;
        }

        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var array = (Array)obj;
            writer.Write(array.Length);
            foreach (var element in array)
            {
                WriteElement(writer, element, elementType);
            }
            return;
        }

        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();
            if (genericDef == typeof(List<>))
            {
                var elementType = type.GetGenericArguments()[0];
                var list = (IList)obj;
                writer.Write(list.Count);
                foreach (var element in list)
                {
                    WriteElement(writer, element, elementType);
                }
                return;
            }
            if (genericDef == typeof(Dictionary<,>))
            {
                var args = type.GetGenericArguments();
                var dict = (IDictionary)obj;
                writer.Write(dict.Count);
                foreach (DictionaryEntry entry in dict)
                {
                    WriteElement(writer, entry.Key, args[0]);
                    WriteElement(writer, entry.Value, args[1]);
                }
                return;
            }
        }

        var definition = Document.Definitions.First(x => x.Name == type.FullName);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var propDef in definition.Properties)
        {
            var property = properties.First(p => p.Name == propDef.Name);
            var value = property.GetValue(obj);
            WriteElement(writer, value, property.PropertyType);
        }
    }

    private void WriteElement(BinaryWriter writer, object value, Type type)
    {
        if (type.IsValueType)
        {
            WriteValue(writer, value, type);
            return;
        }

        if (value == null)
        {
            writer.Write(false);
        }
        else
        {
            writer.Write(true);
            WriteValue(writer, value, type);
        }
    }

    private uint GetTypeId(Type type)
    {
        if (Primitives.Serializers.TryGetValue(type, out var serializer))
        {
            return serializer.Id;
        }

        if (type.IsArray)
        {
            GetTypeId(type.GetElementType()!);
            return 0;
        }

        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();
            if (genericDef == typeof(List<>))
            {
                GetTypeId(type.GetGenericArguments()[0]);
                return 0;
            }
            if (genericDef == typeof(Dictionary<,>))
            {
                var args = type.GetGenericArguments();
                GetTypeId(args[0]);
                GetTypeId(args[1]);
                return 0;
            }
        }

        var existing = Document.Definitions.FirstOrDefault(x => x.Name == type.FullName);
        if (existing != null)
        {
            return existing.Id;
        }

        var typeDefinition = CreateTypeDefinition(type);
        return typeDefinition.Id;
    }

    private SerializedTypeDefinition CreateTypeDefinition(Type type)
    {
        var definition = new SerializedTypeDefinition(NextId, type.FullName);
        Document.Definitions.Add(definition);
        RegisterTypeProperties(type, definition);
        return definition;
    }

    private void RegisterTypeProperties(Type type, SerializedTypeDefinition definition)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        uint propertyId = 0;
        foreach (var property in properties)
        {
            if (!property.CanWrite || !property.CanRead)
            {
                continue;
            }
            var propertyDefinition = new SerializedPropertyDefinition(propertyId++, GetTypeId(property.PropertyType), property.Name);
            definition.Properties.Add(propertyDefinition);
        }
    }
}