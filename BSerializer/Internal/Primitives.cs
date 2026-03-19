using System;
using System.Collections.Generic;
using BSerializer.Internal.Base;
using BSerializer.Internal.Primitive;

namespace BSerializer.Internal;

internal static class Primitives
{
    internal static readonly Dictionary<Type, TypeSerializerBase> Serializers = new()
    {
        { typeof(bool), new BoolSerializer() },
        { typeof(byte), new ByteSerializer() },
        { typeof(sbyte), new SByteSerializer() },
        { typeof(short), new Int16Serializer() },
        { typeof(ushort), new UInt16Serializer() },
        { typeof(int), new Int32Serializer() },
        { typeof(uint), new UInt32Serializer() },
        { typeof(long), new Int64Serializer() },
        { typeof(ulong), new UInt64Serializer() },
        { typeof(float), new FloatSerializer() },
        { typeof(double), new DoubleSerializer() },
        { typeof(decimal), new DecimalSerializer() },
        { typeof(char), new CharSerializer() },
        { typeof(string), new StringSerializer() },
        { typeof(byte[]), new ByteArraySerializer() },
    };
    
    
}