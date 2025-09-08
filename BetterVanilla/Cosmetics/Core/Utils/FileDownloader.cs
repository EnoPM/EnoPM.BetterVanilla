using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BetterVanilla.Cosmetics.Core.Utils;

public static class FileDownloader
{
    private static readonly HttpClient Client = new();
    
    public static async Task DownloadFileAsync(string url, string destinationPath, Action<long, long>? progressCallback = null)
    {
        using var response = await Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
        var canReportProgress = totalBytes != -1;

        await using var contentStream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = File.Create(destinationPath);

        var buffer = new byte[8192];
        long totalRead = 0;
        int bytesRead;

        while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalRead += bytesRead;

            if (canReportProgress && progressCallback != null)
            {
                progressCallback(totalRead, totalBytes);
            }
        }

        if (!canReportProgress && progressCallback != null)
        {
            progressCallback(totalRead, totalRead);
        }
    }
}