using System;
using System.IO;
using BepInEx;

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

    static ModPaths()
    {
        var appDataDirectory = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        if (appDataDirectory == null)
        {
            throw new Exception("Unable to locate appData directory");
        }

        ModDataDirectory = Path.Combine(
            appDataDirectory,
            "LocalLow",
            "EnoPM",
            "BetterVanilla.AmongUs"
        );

        CreateDirectory(ModDataDirectory);
        
        GamePresetsFile = Path.Combine(ModDataDirectory, "GamePresets");
        FeatureCodeFile = Path.Combine(ModDataDirectory, "LocalFeatureCodes");
        OptionsDirectory = Path.Combine(ModDataDirectory, "Options");
        SavedOutfitsFile = Path.Combine(ModDataDirectory, "SavedOutfits");
        PlayerDataFile = Path.Combine(ModDataDirectory, "PlayerData");
        BepInExVersionsDirectory = Path.Combine(Paths.GameRootPath, "BetterVanilla", "ModFiles");
        
        CreateDirectory(OptionsDirectory);
        CreateDirectory(BepInExVersionsDirectory);
    }

    private static void CreateDirectory(string path)
    {
        if (Directory.Exists(path)) return;
        Directory.CreateDirectory(path);
    }
}