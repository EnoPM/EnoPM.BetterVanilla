using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class AssetBundleExtensions
{
    public static T LoadAsset<T>(this AssetBundle assetBundle, string name) where T : Object
    {
        return assetBundle.LoadAsset(name, Il2CppInterop.Runtime.Il2CppType.Of<T>()).As<T>();
    }

    public static T LoadComponent<T>(this AssetBundle assetBundle, string name) where T : MonoBehaviour
    {
        var obj = assetBundle.LoadAsset<GameObject>(name);
        return obj.GetComponent<T>();
    }
}