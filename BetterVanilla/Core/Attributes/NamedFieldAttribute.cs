using System;

namespace BetterVanilla.Core.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class NamedFieldAttribute(string name) : Attribute
{
    public readonly string Name = name;
}