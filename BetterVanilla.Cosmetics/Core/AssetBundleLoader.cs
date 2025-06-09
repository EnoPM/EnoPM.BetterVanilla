using System;
using System.Collections;
using System.IO;
using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core;
using BetterVanilla.Cosmetics.Api.Serialization;
using BetterVanilla.Cosmetics.Utils;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core;

public sealed class AssetBundleLoader
{
    public SerializedResourceFile FileInfos { get; }
    public AssetBundle Bundle => GetAssetBundle();
    public bool IsLoaded => _cache != null;

    private AssetBundle? _cache;
    
    public event Action<long, long>? ProgressChanged;
    public event Action<IResourceFile, AssetBundle>? Loaded; 
    
    public AssetBundleLoader(SerializedResourceFile serializedFile)
    {
        FileInfos = serializedFile;
    }

    private string GetAssetBundleFilePath()
    {
        return Path.Combine(StorageUtility.AssetBundlesDirectory, FileInfos.Name);
    }
    
    private string GetCachedFilePath()
    {
        return Path.Combine(StorageUtility.AssetBundlesDirectory, FileInfos.Name + ".json");
    }

    private SerializedResourceFile? GetCachedFile()
    {
        var filePath = GetCachedFilePath();
        if (!File.Exists(filePath))
        {
            return null;
        }
        var fileContent = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<SerializedResourceFile>(fileContent);
    }

    private void DeleteCachedFile()
    {
        var filePath = GetCachedFilePath();
        if (!File.Exists(filePath)) return;
        File.Delete(filePath);
    }

    private void SaveCachedFile()
    {
        var filePath = GetCachedFilePath();
        File.WriteAllText(filePath, JsonSerializer.Serialize(FileInfos));
    }

    public IEnumerator CoLoad()
    {
        if (_cache != null)
        {
            yield break;
        }
        yield return CoCheckVersion();
        var task = AssetBundle.LoadFromFileAsync(GetAssetBundleFilePath());
        while (!task.isDone)
        {
            yield return null;
        }
        _cache = task.assetBundle;
        Loaded?.Invoke(FileInfos, _cache);
    }

    private IEnumerator CoCheckVersion()
    {
        var cache = GetCachedFile();
        if (cache == null)
        {
            yield return CoDownloadBundle();
        }
        else if (cache.Hash != FileInfos.Hash)
        {
            DeleteCachedFile();
            yield return CoDownloadBundle();
        }
        yield return null;
    }

    private IEnumerator CoDownloadBundle()
    {
        var url = FileInfos.DownloadUrl;
        var destination = GetAssetBundleFilePath();
        
        var task = FileDownloader.DownloadFileAsync(url, destination, OnDownload);
        while (!task.IsCompleted)
        {
            if (task.Exception != null)
            {
                throw task.Exception;
            }
            yield return null;
        }
        SaveCachedFile();
        yield return null;
    }

    private void OnDownload(long downloaded, long total)
    {
        ProgressChanged?.Invoke(downloaded, total);
    }

    private AssetBundle GetAssetBundle()
    {
        if (_cache == null)
        {
            throw new Exception($"AssetBundle {FileInfos.Name} is not loaded.");
        }
        return _cache;
    }
}