using System;
using System.Diagnostics.CodeAnalysis;
using Il2CppInterop.Runtime.InteropTypes;

namespace BetterVanilla.Core.Extensions;

public static class Il2CppObjectBaseExtensions
{
    public static bool Is<T>(this Il2CppObjectBase baseObject, [NotNullWhen(true)] out T? casted)
        where T : Il2CppObjectBase
    {
        casted = baseObject.TryCast<T>();
        return casted != null;
    }

    public static bool Is<T>(this Il2CppObjectBase baseObject) where T : Il2CppObjectBase
    {
        return Is<T>(baseObject, out _);
    }

    public static T As<T>(this Il2CppObjectBase baseObject) where T : Il2CppObjectBase
    {
        if (!baseObject.Is<T>(out var casted))
        {
            throw new Exception($"Unable to cast {baseObject.GetType().Name} to {typeof(T).Name}");
        }
        return casted;
    }
}