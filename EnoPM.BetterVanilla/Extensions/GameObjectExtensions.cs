using System;
using System.Reflection;
using UnityEngine;

namespace EnoPM.BetterVanilla.Extensions;

public static class GameObjectExtensions
{
    private static readonly MethodInfo GameObjectAddComponentGenericMethod;
    
    static GameObjectExtensions()
    {
        GameObjectAddComponentGenericMethod = typeof(GameObject).GetMethod(nameof(GameObject.AddComponent), Type.EmptyTypes);
    }
    
    public static void Destroy(this GameObject gameObject)
    {
        if (!gameObject) return;
        UnityEngine.Object.Destroy(gameObject);
    }
    
    public static void DestroyChildren(this GameObject gameObject)
    {
        for (var i = 0; i < gameObject.transform.GetChildCount(); i++)
        {
            gameObject.transform.GetChild(i).gameObject.Destroy();
        }
    }

    public static T AddComponent<T>(this GameObject gameObject, Type type) where T : Component
    {
        if (!typeof(Component).IsAssignableFrom(type))
        {
            throw new ArgumentException($"The type {type?.FullName} does not inherit from Component and cannot be added to GameObject.", nameof(type));
        }
        var method = GameObjectAddComponentGenericMethod.MakeGenericMethod([type]);
        return (T) method.Invoke(gameObject, null);
    }

    public static Component AddComponent(this GameObject gameObject, Type type) => gameObject.AddComponent<Component>(type);
}