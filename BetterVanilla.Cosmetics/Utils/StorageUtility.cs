using System;
using System.IO;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Utils;

public static class StorageUtility
{
    public static readonly string CosmeticsDirectory;
    public static readonly string HatsDirectory;
    public static readonly string HatsAnimationsDirectory;
    
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