﻿using System;
using System.Reflection;
using EnoPM.BetterVanilla.Core;

namespace EnoPM.BetterVanilla.Data;

public sealed class CustomGameplayButtonData
{
    public readonly bool HasEffect;

    public CustomGameplayButtonData(Type type)
    {
        if (!type.IsAssignableTo(typeof(CustomGameplayButton)))
        {
            throw new ArgumentException($"Type must be assignable to {nameof(CustomGameplayButton)}", nameof(type));
        }
        
        HasEffect = type.GetMethod("CoEffect", BindingFlags.NonPublic | BindingFlags.Instance)!.DeclaringType == type;
    }
}