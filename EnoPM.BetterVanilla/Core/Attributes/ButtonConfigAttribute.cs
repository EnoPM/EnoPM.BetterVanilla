using System;
using EnoPM.BetterVanilla.Core.Data;

namespace EnoPM.BetterVanilla.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ButtonConfigAttribute(ButtonPositions position) : Attribute
{
    public readonly ButtonPositions Position = position;
}