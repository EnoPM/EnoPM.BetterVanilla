using System;
using System.IO;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Utils;

public static class StorageUtility
{
    public static readonly string CosmeticsDirectory;
    
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
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        CosmeticsDirectory = directory;
    }
}