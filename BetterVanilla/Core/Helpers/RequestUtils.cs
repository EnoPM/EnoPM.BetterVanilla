using System;
using System.Collections;
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

    public static IEnumerator CoGet<T>(string url, Action<T> onResult, Action<Exception>? onException = null, JsonSerializerOptions? options = null) where T : class
    {
        var requestTask = GetAsync<T>(url, options);
        while (!requestTask.IsCompleted)
        {
            if (requestTask.Exception != null)
            {
                onException?.Invoke(requestTask.Exception);
                yield break;
            }
            yield return null;
        }
        if (requestTask.Result == null)
        {
            onException?.Invoke(new Exception($"Unable to get result: {url}"));
            yield break;
        }
        onResult.Invoke(requestTask.Result);
    }

    public static async Task<T?> GetAsync<T>(string url, JsonSerializerOptions? options = null) where T : class
    {
        Ls.LogMessage($"URL: {url}");
        using var response = await UserClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, options);
    }

    public static IEnumerator CoDownloadFile(string url, string destinationPath, IProgress<float>? progress = null)
    {
        var downloadTask = DownloadFileAsync(url, destinationPath, progress);
        while (!downloadTask.IsCompleted)
        {
            if (downloadTask.Exception != null)
            {
                Ls.LogWarning(downloadTask.Exception.Message);
                break;
            }
            yield return null;
        }
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