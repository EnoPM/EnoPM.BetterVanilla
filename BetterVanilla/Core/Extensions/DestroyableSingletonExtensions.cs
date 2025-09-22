using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class DestroyableSingletonExtensions
{
    public static void BaseAwake<T>(this T singleton) where T : DestroyableSingleton<T>
    {
        if (DestroyableSingleton<T>._instance == null)
        {
            DestroyableSingleton<T>._instance = singleton;
            if (!singleton.DontDestroy) return;
            Object.DontDestroyOnLoad(singleton.gameObject);
        }
        else
        {
            if (DestroyableSingleton<T>._instance != singleton) return;
            Object.Destroy(singleton.gameObject);
        }
    }

    public static void BaseOnDestroy<T>(this T singleton) where T : DestroyableSingleton<T>
    {
        if (singleton.DontDestroy || DestroyableSingleton<T>._instance != singleton) return;
        DestroyableSingleton<T>._instance = null!;
    }
}