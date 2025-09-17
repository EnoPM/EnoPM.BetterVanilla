using System;
using System.IO;
using BepInEx;
using UnityEngine;

namespace BetterVanilla.Core;

public static class ModPaths
{
    public static string ModDataDirectory { get; }
    public static string GamePresetsFile { get; }
    public static string FeatureCodeFile { get; }
    public static string OptionsDirectory { get; }
    public static string SavedOutfitsFile { get; }
    public static string PlayerDataFile { get; }
    public static string BepInExVersionsDirectory { get; }
    public static string CosmeticsDirectory { get; }
    public static string CosmeticsBundlesDirectory { get; }
    public static string CosmeticsLocalBundlesDirectory { get; }

    static ModPaths()
    {
        var appDirectory = Application.persistentDataPath;

        ModDataDirectory = Path.Combine(
            appDirectory,
            "EnoPM",
            "BetterVanilla.AmongUs"
        );
        
        CreateDirectory(ModDataDirectory);
        
#if !ANDROID
        var appDataDirectory = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        if (appDataDirectory == null)
        {
            throw new Exception("Unable to locate appData directory");
        }
        var legacyModDataDirectory = Path.Combine(
            appDataDirectory,
            "LocalLow",
            "EnoPM",
            "BetterVanilla.AmongUs"
        );
        if (Directory.Exists(legacyModDataDirectory))
        {
            foreach (var filePath in Directory.GetFiles(legacyModDataDirectory))
            {
                var fileName = Path.GetFileName(filePath);
                var destinationPath = Path.Combine(ModDataDirectory, fileName);
                if (!File.Exists(destinationPath))
                {
                    File.Move(filePath, destinationPath);
                }
            }
            foreach (var directoryPath in Directory.GetDirectories(legacyModDataDirectory))
            {
                var directoryName = Path.GetFileName(directoryPath);
                var destinationPath = Path.Combine(ModDataDirectory, directoryName);
                if (!Directory.Exists(destinationPath))
                {
                    Directory.Move(directoryPath, destinationPath);
                }
            }
            
            Directory.Delete(legacyModDataDirectory, true);
        }
#endif
        
        GamePresetsFile = Path.Combine(ModDataDirectory, "GamePresets");
        FeatureCodeFile = Path.Combine(ModDataDirectory, "LocalFeatureCodes");
        OptionsDirectory = Path.Combine(ModDataDirectory, "Options");
        SavedOutfitsFile = Path.Combine(ModDataDirectory, "SavedOutfits");
        PlayerDataFile = Path.Combine(ModDataDirectory, "PlayerData");
        BepInExVersionsDirectory = Path.Combine(Paths.GameRootPath, "BetterVanilla", "BepInExVersions");
        CosmeticsDirectory = Path.Combine(ModDataDirectory, "Cosmetics");
        CosmeticsBundlesDirectory = Path.Combine(CosmeticsDirectory, "Bundles");
        CosmeticsLocalBundlesDirectory = Path.Combine(CosmeticsDirectory, "LocalBundles");
        
        CreateDirectory(CosmeticsDirectory);
        CreateDirectory(CosmeticsBundlesDirectory);
        CreateDirectory(CosmeticsLocalBundlesDirectory);
        CreateDirectory(OptionsDirectory);
        CreateDirectory(BepInExVersionsDirectory);
    }

    private static void CreateDirectory(string path)
    {
        if (Directory.Exists(path)) return;
        Directory.CreateDirectory(path);
    }
}