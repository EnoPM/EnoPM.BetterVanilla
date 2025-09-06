using System.IO.Compression;
using BetterVanilla.Installer.Serialization;
using BetterVanilla.Installer.Utils;

namespace BetterVanilla.Installer;

public sealed class ModInstaller
{
    private bool AllowPrerelease { get; }
    private PathsUtility Paths { get; }

    public ModInstaller(string gameDirectoryPath)
    {
        Paths = new PathsUtility(gameDirectoryPath);
        AllowPrerelease = false;
    }

    public async Task StartAsync()
    {
        var bepInExVersion = await RequestUtility.GetBepInExVersionAsync();
        var versionDirectory = Paths.GetBepInExDirectory(bepInExVersion);
        if (!Directory.Exists(versionDirectory))
        {
            await InstallBepInExAsync(bepInExVersion, versionDirectory);
        }
        
        UpdateGameDirectory(versionDirectory);
        
        var release = await GetLatestReleaseAsync();
        if (release == null)
        {
            throw new Exception("Unable to find latest release");
        }

        await InstallReleaseAsync(versionDirectory, release);

        if (!File.Exists(Paths.SteamAppIdFilePath))
        {
            await File.WriteAllTextAsync(Paths.SteamAppIdFilePath, Constants.SteamAppId.ToString());
        }
    }

    private async Task InstallReleaseAsync(string versionDirectory, Release release)
    {
        ConsoleUtility.WriteLine(ConsoleColor.Cyan, $"Installing {release.Name} release files");
        var pluginsDirectory = Path.Combine(versionDirectory, Paths.BepInExPluginsDirectoryRelativePath);
        foreach (var asset in release.Assets.Where(IsValidAsset))
        {
            ConsoleUtility.WriteLine(ConsoleColor.Cyan, $"- Installing {asset.Name}");
            var destination = Path.Combine(pluginsDirectory, asset.Name);
            await RequestUtility.DownloadAssetAsync(asset, destination);
        }
    }

    private async Task InstallBepInExAsync(BepInExVersion version, string versionDirectory)
    {
        ConsoleUtility.WriteLine(ConsoleColor.Cyan, "Installing BepInEx");
        await using var stream = new MemoryStream();
        await RequestUtility.DownloadBepInExArchiveAsync(version, stream);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        Directory.CreateDirectory(versionDirectory);
        archive.ExtractToDirectory(versionDirectory);
        DeletePreviousBepInExInstallations(versionDirectory);
        
        Directory.CreateDirectory(Path.Combine(versionDirectory, Paths.BepInExConfigDirectoryRelativePath));
        await RequestUtility.DownloadBepInExConfigFileAsync(Path.Combine(versionDirectory, Paths.BepInExConfigFileRelativePath));
        Console.WriteLine();
    }

    private void DeletePreviousBepInExInstallations(string versionDirectory)
    {
        if (Directory.Exists(Paths.LegacyBepInExDirectoryPath))
        {
            Directory.Delete(Paths.LegacyBepInExDirectoryPath, true);
        }
        
        if (Directory.Exists(Paths.LegacyDotnetDirectoryPath))
        {
            Directory.Delete(Paths.LegacyDotnetDirectoryPath, true);
        }
        
        foreach (var directory in Directory.GetDirectories(Paths.BepInExVersionsDirectoryPath))
        {
            if (directory == versionDirectory) continue;
            Directory.Delete(directory, true);
        }
    }
    
    private void UpdateGameDirectory(string versionDirectory)
    {
        if (!File.Exists(Paths.WinHttpDllFilePath))
        {
            File.Copy(
                Path.Combine(versionDirectory, DoorstopUtility.EntryPointFilename),
                Paths.WinHttpDllFilePath
            );
        }

        if (!File.Exists(Paths.DoorstopConfigFilePath))
        {
            File.Copy(
                Path.Combine(versionDirectory, DoorstopUtility.DoorstopConfigFilename),
                Paths.DoorstopConfigFilePath
            );
        }
        
        DoorstopUtility.UpdateDoorstopConfigFile(Paths, versionDirectory);
    }

    private async Task<Release?> GetLatestReleaseAsync()
    {
        var releases = await RequestUtility.GetReleasesAsync();
        return releases.FirstOrDefault(IsValidRelease);
    }

    private bool IsValidRelease(Release release)
    {
        var validAssets = release.Assets.Count(IsValidAsset);
        if (validAssets == 0) return false;
        if (!AllowPrerelease)
        {
            return !release.Prerelease;
        }
        return true;
    }

    private static bool IsValidAsset(Asset asset) => asset.Name.EndsWith(".dll");
}