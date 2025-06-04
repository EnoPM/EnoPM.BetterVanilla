using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.Json;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Cosmetics.Data;
using BetterVanilla.Cosmetics.Utils;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace BetterVanilla.Cosmetics.Components;

public class CosmeticsLoader(IntPtr ptr) : MonoBehaviour(ptr)
{
    static CosmeticsLoader()
    {
        ClassInjector.RegisterTypeInIl2Cpp<CosmeticsLoader>();
    }
    
    private bool IsRunning { get; set; }

    public void FetchCosmetics(string repository, string manifestFileName, string? customDirectory)
    {
        if (IsRunning || customDirectory == null) return;
        this.StartCoroutine(CoFetchCosmetics(repository, manifestFileName, customDirectory));
    }

    [HideFromIl2Cpp]
    private IEnumerator CoFetchCosmetics(string repository, string manifestFileName, string customDirectory)
    {
        IsRunning = true;
        var url = $"https://raw.githubusercontent.com/{repository}/master/{manifestFileName}";
        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        CosmeticsPlugin.Logging.LogMessage($"Download manifest at: {url}");
        www.SetUrl(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.isNetworkError || www.isHttpError)
        {
            CosmeticsPlugin.Logging.LogError(www.error);
            yield break;
        }

        var response = JsonSerializer.Deserialize<ManifestFile>(www.downloadHandler.text, new JsonSerializerOptions
        {
            AllowTrailingCommas = true
        });
        www.downloadHandler.Dispose();
        www.Dispose();

        if (response != null)
        {
            yield return CoDownloadHats(response, repository, customDirectory);
        }

        IsRunning = false;
    }

    private static IEnumerator CoDownloadHats(ManifestFile response, string repository, string? hatsDirectory)
    {
        if (hatsDirectory == null) yield break;
        if (!Directory.Exists(hatsDirectory))
        {
            Directory.CreateDirectory(hatsDirectory);
        }
        CosmeticsManager.UnregisteredHats.AddRange(CosmeticsManager.SanitizeHats(response, hatsDirectory));
        var toDownload = CosmeticsManager.GenerateDownloadList(CosmeticsManager.UnregisteredHats, hatsDirectory, out var totalFileCount);
        CosmeticsPlugin.Logging.LogMessage($"{toDownload.Count} hat asset files to download");
        if (toDownload.Count <= 0) yield break;
        var tracker = new DownloadTracker();
        yield return tracker.CoInit(totalFileCount, totalFileCount - toDownload.Count);
        foreach (var fileName in toDownload)
        {
            yield return tracker.CoPauseDownloadIfRequired();
            yield return CoDownloadAssetFile($"https://raw.githubusercontent.com/{repository}/master/hats/{Uri.EscapeDataString(fileName)}", Path.Combine(hatsDirectory, fileName));
            yield return tracker.CoIncrementDownloadCount();
            CosmeticsManager.OnHatFileDownloaded(fileName);
        }

        yield return new WaitForSeconds(2f);
        yield return tracker.CoUnload();
    }

    private static IEnumerator CoDownloadAssetFile(string fileUrl, string destination)
    {
        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        www.SetUrl(fileUrl);
        var handler = new DownloadHandlerBuffer();
        www.downloadHandler = handler;
        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.isNetworkError || www.isHttpError)
        {
            CosmeticsPlugin.Logging.LogError($"{www.error}: {fileUrl}");
            yield break;
        }
        
        CosmeticsPlugin.Logging.LogMessage($"Download asset file at: {fileUrl} to {destination}");
        var nativeData = handler.GetNativeData();

        var persistTask = File.WriteAllBytesAsync(destination, nativeData.ToArray());
        while (!persistTask.IsCompleted)
        {
            if (persistTask.Exception != null)
            {
                CosmeticsPlugin.Logging.LogError(persistTask.Exception.Message);
                break;
            }

            yield return new WaitForEndOfFrame();
        }
        

        www.downloadHandler.Dispose();
        www.Dispose();
    }
}