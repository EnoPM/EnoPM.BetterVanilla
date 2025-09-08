using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core.Utils;

public static class StorageUtility
{
    public static readonly string CosmeticsDirectory;
    public static readonly string HatsDirectory;
    public static readonly string HatsAnimationsDirectory;
    public static readonly string AssetBundlesDirectory;
    public static readonly string AssetBundleManifestFile;
    
    static StorageUtility()
    {
        var appDataDirectory = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        if (appDataDirectory == null)
        {
            throw new Exception("Unable to locate appData directory");
        }
        
        var directory = Path.Combine(
            appDataDirectory,
            "LocalLow",
            Application.companyName,
            Application.productName,
            "BetterVanilla",
            "Cosmetics"
        );
        
        CosmeticsDirectory = directory;
        CreateDirectoryIfNotExists(CosmeticsDirectory);
        
        AssetBundlesDirectory = Path.Combine(CosmeticsDirectory, "AssetBundles");
        CreateDirectoryIfNotExists(AssetBundlesDirectory);
        AssetBundleManifestFile = Path.Combine(AssetBundlesDirectory, "Manifest.json");
        
        HatsDirectory = Path.Combine(CosmeticsDirectory, "Hats");
        CreateDirectoryIfNotExists(HatsDirectory);
        
        HatsAnimationsDirectory = Path.Combine(HatsDirectory, "Animations");
        CreateDirectoryIfNotExists(HatsAnimationsDirectory);
    }

    private static void CreateDirectoryIfNotExists(string directoryPath)
    {
        if (Directory.Exists(directoryPath)) return;
        Directory.CreateDirectory(directoryPath);
    }
}