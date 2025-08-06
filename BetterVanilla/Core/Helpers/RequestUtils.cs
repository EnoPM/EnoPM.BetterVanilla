using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BetterVanilla.Core.Helpers;

public static class RequestUtils
{
    private static HttpClient UserClient { get; }
    private static HttpClient Client { get; }
    
    static RequestUtils()
    {
        Client = new HttpClient();
        UserClient = new HttpClient();
        UserClient.DefaultRequestHeaders.UserAgent.ParseAdd("BetterVanillaUpdater/1.0");
    }

    public static async Task<T?> GetAsync<T>(string url, JsonSerializerOptions? options = null)
    {
        using var response = await UserClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, options);
    }

    public static async Task DownloadFileAsync(string url, string destinationPath, IProgress<float>? progress = null)
    {
        using var response = await Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        
        var contentLength = response.Content.Headers.ContentLength ?? -1L;
        var totalBytesRead = 0L;
        var buffer = new byte[8192];
        var isMoreToRead = true;
        
        await using var contentStream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

        do
        {
            var bytesRead = await contentStream.ReadAsync(buffer);
            if (bytesRead == 0)
            {
                isMoreToRead = false;
                progress?.Report(1f); // 100%
                continue;
            }

            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));

            totalBytesRead += bytesRead;
            if (contentLength == -1) continue;
            var percentage = (float)totalBytesRead / contentLength;
            progress?.Report(percentage);
        } while (isMoreToRead);
    }
}