using System.Text.Json;
using BetterVanilla.ToolsLib.Serialization;
using BetterVanilla.ToolsLib.Utils;
using GithubJsonContext = BetterVanilla.ToolsLib.Serialization.GithubJsonContext;

namespace BetterVanilla.Installer.Utils;

public static class RequestUtility
{
    private const string GithubReleasesApiUrl = $"https://api.github.com/repos/{Constants.GithubRepository}/releases";
    private const string GithubRepositoryBaseUrl = $"https://raw.githubusercontent.com/{Constants.GithubRepository}/refs/heads/{Constants.GithubBranch}";
    private const string BepInExVersionUrl = $"{GithubRepositoryBaseUrl}/{Constants.BepInExVersionFilename}";
    private const string BepInExConfigUrl = $"{GithubRepositoryBaseUrl}/{Constants.BepInExConfigFilename}";

    private static HttpClient UserClient { get; }
    private static HttpClient Client { get; }

    static RequestUtility()
    {
        Client = new HttpClient();
        UserClient = new HttpClient();
        UserClient.DefaultRequestHeaders.UserAgent.ParseAdd(Constants.UserAgent);
    }

    public static async Task<Release[]> GetReleasesAsync()
    {
        await using var stream = await UserClient.GetStreamAsync(GithubReleasesApiUrl);
        return await JsonSerializer.DeserializeAsync(stream, GithubJsonContext.Default.ReleaseArray) ?? [];
    }

    public static async Task<BepInExVersion> GetBepInExVersionAsync()
    {
        await using var stream = await Client.GetStreamAsync(BepInExVersionUrl);
        var result = await JsonSerializer.DeserializeAsync(stream, GithubJsonContext.Default.BepInExVersion);
        if (result == null)
        {
            throw new Exception("Unable to retrieve BepInEx version file");
        }
        return result;
    }

    public static async Task DownloadBepInExConfigFileAsync(string destinationPath)
    {
        await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
        await using var stream = await Client.GetStreamAsync(BepInExConfigUrl);
        await stream.CopyToAsync(fileStream);
    }

    public static async Task DownloadBepInExArchiveAsync(BepInExVersion version, Stream destination)
    {
        var url = $"https://builds.bepinex.dev/projects/bepinex_be/{version.BuildNumber}/BepInEx-Unity.IL2CPP-win-x86-{version.Version.Major}.{version.Version.Minor}.{version.Version.Build}-be.{version.BuildNumber}+{version.BuildHash}.zip";
        await DownloadFileAsync(url, destination);
        destination.Position = 0;
    }

    public static async Task DownloadAssetAsync(Asset asset, string destinationPath)
    {
        await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
        await DownloadFileAsync(asset.DownloadUrl, fileStream);
    }

    private static async Task DownloadFileAsync(string url, Stream destination)
    {
        using var response = await Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var contentLength = response.Content.Headers.ContentLength ?? -1L;
        var totalBytesRead = 0L;
        var buffer = new byte[8192];
        var isMoreToRead = true;

        await using var contentStream = await response.Content.ReadAsStreamAsync();
        do
        {
            var bytesRead = await contentStream.ReadAsync(buffer);
            if (bytesRead == 0)
            {
                isMoreToRead = false;
                ConsoleUtility.WriteConsoleProgress(1.0);
                continue;
            }

            await destination.WriteAsync(buffer.AsMemory(0, bytesRead));

            totalBytesRead += bytesRead;
            if (contentLength == -1) continue;
            var percentage = (double)totalBytesRead / contentLength;
            ConsoleUtility.WriteConsoleProgress(percentage);
        } while (isMoreToRead);
        Console.WriteLine();
    }
}