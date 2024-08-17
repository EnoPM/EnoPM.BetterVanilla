using System;
using EnoPM.BetterVanilla.Data;

namespace EnoPM.BetterVanilla.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ButtonConfigAttribute(ButtonPositions position) : Attribute
{
    public readonly ButtonPositions Position = position;
}