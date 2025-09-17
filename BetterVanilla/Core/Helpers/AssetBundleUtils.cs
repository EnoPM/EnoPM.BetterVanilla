using System;
using System.Reflection;
using Cpp2IL.Core.Extensions;
using UnityEngine;

namespace BetterVanilla.Core.Helpers;

public static class AssetBundleUtils
{
    public static AssetBundle LoadFromExecutingAssembly(string assetBundleName)
    {
        #if ANDROID
        assetBundleName = $"{assetBundleName}.android";
        #endif
        var assembly = Assembly.GetExecutingAssembly();
        var resourceStream = assembly.GetManifestResourceStream(assetBundleName);
        if (resourceStream == null)
        {
            throw new Exception($"Unable to find resource: {assetBundleName} in assembly {assembly.FullName}");
        }
        return AssetBundle.LoadFromMemory(resourceStream.ReadBytes());
    }
}