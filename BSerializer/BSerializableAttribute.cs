using System;

namespace BSerializer;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class BSerializableAttribute(Type type) : Attribute
{
    public Type Type { get; } = type;
}
