using System;

namespace EnoPM.BetterVanilla.Core.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class DisplayAsAttribute(string displayName) : Attribute
{
    public readonly string DisplayName = displayName;
}