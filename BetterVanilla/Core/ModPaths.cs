using System;
using System.IO;

namespace BetterVanilla.Core;

public static class ModPaths
{
    public static string ModDataDirectory { get; }

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

        if (Directory.Exists(ModDataDirectory)) return;
        Directory.CreateDirectory(ModDataDirectory);
    }
}