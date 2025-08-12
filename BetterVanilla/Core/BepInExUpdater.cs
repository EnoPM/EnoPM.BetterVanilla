using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using BepInEx;
using BetterVanilla.Core.Helpers;

namespace BetterVanilla.Core;

public sealed class BepInExUpdater
{
    public const string GithubRepository = "EnoPM/EnoPM.BetterVanilla";
    private const string ConfigFileDownloadUrl = $"https://raw.githubusercontent.com/{GithubRepository}/refs/heads/master/BepInEx.cfg";
    
    private string BepInExDownloadUrl { get; }
    
    public BepInExUpdater(Version version, uint buildNumber, string buildHash)
    {
        BepInExDownloadUrl = $"https://builds.bepinex.dev/projects/bepinex_be/{buildNumber}/BepInEx-Unity.IL2CPP-win-x86-{version.Major}.{version.Minor}.{version.Build}-be.{buildNumber}+{buildHash}.zip";
    }

    public IEnumerator CoUpdateIfNecessary(IProgress<float>? progress = null)
    {
        if (!IsUpdateRequired())
        {
            yield break;
        }
        yield return CoInstallBepInEx(progress);
    }
    
    private IEnumerator CoInstallBepInEx(IProgress<float>? progress = null)
    {
        var tempFile = Path.GetTempFileName();
        yield return RequestUtils.CoDownloadFile(BepInExDownloadUrl, tempFile, progress);
        if (!File.Exists(tempFile)) yield break;
        
        InitDirectory();
        ExtractArchive(tempFile);
        File.Delete(tempFile);
        if (!Directory.Exists(ModPaths.CurrentBepInExDirectory)) yield break;
        
        var configDirectory = Path.Combine(ModPaths.CurrentBepInExDirectory, "BepInEx", "config");
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }
        var configFile = Path.Combine(configDirectory, "BepInEx.cfg");
        yield return RequestUtils.CoDownloadFile(ConfigFileDownloadUrl, configFile, progress);
        if (!File.Exists(configFile)) yield break;

        UpdateDoorstopConfig();
        SaveBepInExVersion();
    }
    
    private bool IsUpdateRequired()
    {
        if (!File.Exists(ModPaths.BepInExVersionFile))
        {
            return true;
        }
        using var file = File.OpenRead(ModPaths.BepInExVersionFile);
        using var reader = new BinaryReader(file);
        var downloadUrl = reader.ReadString();
        return downloadUrl != BepInExDownloadUrl;
    }
    
    private void SaveBepInExVersion()
    {
        using var file = File.Create(ModPaths.BepInExVersionFile);
        using var writer = new BinaryWriter(file);
        writer.Write(BepInExDownloadUrl);
    }

    private static void UpdateDoorstopConfig()
    {
        var filePath = Path.Combine(Paths.GameRootPath, "doorstop_config.ini");
        var text = File.ReadAllText(filePath);
        var rows = text.Split('\n');
        for (var i = 0; i < rows.Length; i++)
        {
            if (rows[i].StartsWith("target_assembly ="))
            {
                rows[i] = $"target_assembly = {Path.Combine(ModPaths.CurrentBepInExDirectory, "BepInEx", "core", "BepInEx.Unity.IL2CPP.dll")}";
                continue;
            }
            if (rows[i].StartsWith("coreclr_path ="))
            {
                rows[i] = $"coreclr_path = {Path.Combine(ModPaths.CurrentBepInExDirectory, "dotnet", "coreclr.dll")}";
                continue;
            }
            if (rows[i].StartsWith("coreclr_path ="))
            {
                rows[i] = $"corlib_dir = {Path.Combine(ModPaths.CurrentBepInExDirectory, "dotnet")}";
            }
        }
        File.WriteAllText(filePath, string.Join('\n', rows));
    }

    private static void ExtractArchive(string archivePath)
    {
        using var file = File.OpenRead(archivePath);
        using var archive = new ZipArchive(file, ZipArchiveMode.Read);
        Directory.CreateDirectory(ModPaths.CurrentBepInExDirectory);
        archive.ExtractToDirectory(ModPaths.CurrentBepInExDirectory);
    }

    private static void InitDirectory()
    {
        if (Directory.Exists(ModPaths.PreviousBepInExDirectory))
        {
            Directory.Delete(ModPaths.PreviousBepInExDirectory, true);
        }
        if (!Directory.Exists(ModPaths.CurrentBepInExDirectory)) return;
        Directory.Move(ModPaths.CurrentBepInExDirectory, ModPaths.PreviousBepInExDirectory);
    }
}