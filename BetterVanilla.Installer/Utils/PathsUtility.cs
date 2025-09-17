using BetterVanilla.ToolsLib.Serialization;
using BetterVanilla.ToolsLib.Utils;

namespace BetterVanilla.Installer.Utils;

public sealed class PathsUtility
{
    private string GameDirectoryPath { get; }
    private string ModDirectoryPath { get; }
    public string BepInExVersionsDirectoryPath { get; }
    public string WinHttpDllFilePath { get; }
    public string DoorstopConfigFilePath { get; }
    public string SteamAppIdFilePath { get; }
    public string LegacyBepInExDirectoryPath { get; }
    public string LegacyDotnetDirectoryPath { get; }
    public string BepInExPluginsDirectoryRelativePath { get; }
    public string BepInExConfigDirectoryRelativePath { get; }
    public string BepInExConfigFileRelativePath { get; }

    public PathsUtility(string gameDirectoryPath)
    {
        GameDirectoryPath = gameDirectoryPath;

        ModDirectoryPath = Path.Combine(GameDirectoryPath, Constants.ModDirectoryName);
        if (!Directory.Exists(ModDirectoryPath))
        {
            Directory.CreateDirectory(ModDirectoryPath);
        }

        BepInExVersionsDirectoryPath = Path.Combine(ModDirectoryPath, "BepInExVersions");
        if (!Directory.Exists(BepInExVersionsDirectoryPath))
        {
            Directory.CreateDirectory(BepInExVersionsDirectoryPath);
        }
        
        WinHttpDllFilePath = Path.Combine(GameDirectoryPath, DoorstopUtility.EntryPointFilename);
        DoorstopConfigFilePath = Path.Combine(GameDirectoryPath, DoorstopUtility.DoorstopConfigFilename);
        SteamAppIdFilePath = Path.Combine(GameDirectoryPath, "steam_appid.txt");
        
        LegacyBepInExDirectoryPath = Path.Combine(GameDirectoryPath, "BepInEx");
        LegacyDotnetDirectoryPath = Path.Combine(GameDirectoryPath, "dotnet");
        BepInExPluginsDirectoryRelativePath = Path.Combine("BepInEx", "plugins");
        BepInExConfigDirectoryRelativePath = Path.Combine("BepInEx", "config");
        BepInExConfigFileRelativePath = Path.Combine(BepInExConfigDirectoryRelativePath, Constants.BepInExConfigFilename);
    }

    public string GetBepInExDirectory(BepInExVersion version) => Path.Combine(BepInExVersionsDirectoryPath, $"{version.Version.Major}.{version.Version.Minor}.{version.Version.Build}-{version.BuildNumber}-{version.BuildHash}");
}