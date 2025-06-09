using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Cosmetics.Api.Core;
using BetterVanilla.Cosmetics.Api.Serialization;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core;

public sealed class CosmeticsLoader
{
    private SerializedCosmeticsManifest Manifest { get; }

    private List<AssetBundleLoader> BundleLoaders { get; } = [];
    private List<HatLoader> HatLoaders { get; } = [];

    public CosmeticsLoader(SerializedCosmeticsManifest manifest)
    {
        Manifest = manifest;
        if (Manifest.Bundles != null)
        {
            BundleLoaders.AddRange(Manifest.Bundles.Select(Deserialize));
        }
        if (Manifest.Hats != null)
        {
            HatLoaders.AddRange(Manifest.Hats.Select(Deserialize));
        }
    }

    public void LoadCosmetics()
    {
        
    }

    private void OnAssetBundleLoaded(IResourceFile file, AssetBundle assetBundle)
    {
        // TODO: Load cosmetic sprites
    }

    private AssetBundleLoader Deserialize(SerializedResourceFile serializedResourceFile)
    {
        var loader = new AssetBundleLoader(serializedResourceFile);
        loader.Loaded += OnAssetBundleLoaded;
        return loader;
    }
    
    private static HatLoader Deserialize(SerializedHat serializedHat)
    {
        return new HatLoader(serializedHat);
    }
}