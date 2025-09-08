using System.Collections;
using BetterVanilla.Core;

namespace BetterVanilla.Cosmetics.Core.Utils;

public class DownloadTracker
{
    public int FileCount { get; private set; }
    public int DownloadFileCount { get; private set; }
    public string Title { get; private set; } = string.Empty;

    public IEnumerator CoInit(int fileCount, int downloadFileCount, string title = "")
    {
        FileCount = fileCount;
        DownloadFileCount = downloadFileCount;
        Title = title;
        Ls.LogMessage($"Init: {DownloadFileCount}/{FileCount}");
        yield return null;
    }

    public IEnumerator CoIncrementDownloadCount()
    {
        DownloadFileCount++;
        Ls.LogMessage($"Increment: {DownloadFileCount}/{FileCount}");
        yield return null;
    }

    public IEnumerator CoPauseDownloadIfRequired()
    {
        yield return null;
    }

    public IEnumerator CoUnload()
    {
        yield return null;
    }
}