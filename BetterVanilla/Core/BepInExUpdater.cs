using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using BepInEx;
using BetterVanilla.Core.Helpers;
using UnityEngine;

namespace BetterVanilla.Core;

public sealed class BepInExUpdater
{
    public const string GithubRepository = "EnoPM/EnoPM.BetterVanilla";
    private const string ConfigFileDownloadUrl = $"https://raw.githubusercontent.com/{GithubRepository}/refs/heads/master/BepInEx.cfg";
    
    private string BepInExDownloadUrl { get; }
    
    public string CurrentBepInExDirectory { get; }
    
    public BepInExUpdater(Version version, uint buildNumber, string buildHash)
    {
        BepInExDownloadUrl = $"https://builds.bepinex.dev/projects/bepinex_be/{buildNumber}/BepInEx-Unity.IL2CPP-win-x86-{version.Major}.{version.Minor}.{version.Build}-be.{buildNumber}+{buildHash}.zip";
        CurrentBepInExDirectory = Path.Combine(ModPaths.BepInExVersionsDirectory, $"{version.Major}.{version.Minor}.{version.Build}-{buildNumber}-{buildHash}");
    }

    public IEnumerator CoUpdateIfNecessary(IProgress<float>? progress = null)
    {
        DeleteOldBepInExVersions();
        if (Directory.Exists(CurrentBepInExDirectory))
        {
            yield break;
        }
        yield return CoInstallBepInEx(progress);
        yield return new WaitForSeconds(2f);
    }
    
    private IEnumerator CoInstallBepInEx(IProgress<float>? progress = null)
    {
        var tempFile = Path.GetTempFileName();
        yield return RequestUtils.CoDownloadFile(BepInExDownloadUrl, tempFile, progress);
        if (!File.Exists(tempFile)) yield break;
        
        ExtractArchive(tempFile);
        File.Delete(tempFile);
        if (!Directory.Exists(CurrentBepInExDirectory)) yield break;
        
        var configDirectory = Path.Combine(CurrentBepInExDirectory, "BepInEx", "config");
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }
        var configFile = Path.Combine(configDirectory, "BepInEx.cfg");
        yield return RequestUtils.CoDownloadFile(ConfigFileDownloadUrl, configFile, progress);
        if (!File.Exists(configFile)) yield break;

        UpdateDoorstopConfig();
    }

    private void UpdateDoorstopConfig()
    {
        var filePath = Path.Combine(Paths.GameRootPath, "doorstop_config.ini");
        var text = File.ReadAllText(filePath);
        var rows = text.Split('\n');
        for (var i = 0; i < rows.Length; i++)
        {
            if (rows[i].StartsWith("target_assembly ="))
            {
                rows[i] = $"target_assembly = {Path.Combine(CurrentBepInExDirectory, "BepInEx", "core", "BepInEx.Unity.IL2CPP.dll")}";
                continue;
            }
            if (rows[i].StartsWith("coreclr_path ="))
            {
                rows[i] = $"coreclr_path = {Path.Combine(CurrentBepInExDirectory, "dotnet", "coreclr.dll")}";
                continue;
            }
            if (rows[i].StartsWith("corlib_dir ="))
            {
                rows[i] = $"corlib_dir = {Path.Combine(CurrentBepInExDirectory, "dotnet")}";
            }
        }
        File.WriteAllText(filePath, string.Join('\n', rows));
    }

    private void ExtractArchive(string archivePath)
    {
        using var file = File.OpenRead(archivePath);
        using var archive = new ZipArchive(file, ZipArchiveMode.Read);
        Directory.CreateDirectory(CurrentBepInExDirectory);
        archive.ExtractToDirectory(CurrentBepInExDirectory);
    }

    private void DeleteOldBepInExVersions()
    {
        foreach (var directory in Directory.GetDirectories(ModPaths.BepInExVersionsDirectory))
        {
            if (directory == CurrentBepInExDirectory || directory == Path.GetDirectoryName(Paths.BepInExRootPath)) continue;
            Ls.LogMessage($"Directory to delete: {directory}");
            Directory.Delete(directory, true);
        }
    }
}