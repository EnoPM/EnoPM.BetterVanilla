using System;
using System.IO;

namespace BetterVanilla.Core;

public static class ModPaths
{
    public static string ModDataDirectory { get; }
    public static string GamePresetsFile { get; }
    public static string FeatureCodeFile { get; }
    public static string OptionsDirectory { get; }
    public static string SavedOutfitsFile { get; }
    public static string PlayerDataFile { get; }
    public static string BepInExContentDirectory { get; }
    public static string BepInExVersionFile { get; }
    public static string CurrentBepInExDirectory { get; }
    public static string PreviousBepInExDirectory { get; }

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
        BepInExContentDirectory = Path.Combine(ModDataDirectory, "BepInExFiles");
        BepInExVersionFile = Path.Combine(BepInExContentDirectory, "CurrentBepInExVersion");
        CurrentBepInExDirectory = Path.Combine(BepInExContentDirectory, "Current");
        PreviousBepInExDirectory = Path.Combine(BepInExContentDirectory, "Previous");
        
        CreateDirectory(OptionsDirectory);
        CreateDirectory(BepInExContentDirectory);
    }

    private static void CreateDirectory(string path)
    {
        if (Directory.Exists(path)) return;
        Directory.CreateDirectory(path);
    }
}