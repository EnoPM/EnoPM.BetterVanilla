using System;
using System.Collections;
using System.IO;
using System.Linq;
using BetterVanilla.Core;
using BetterVanilla.Core.Helpers;
using BetterVanilla.Cosmetics;
using BetterVanilla.Cosmetics.Api.Core.Bundle;
using BetterVanilla.Cosmetics.Core.Data;
using UnityEngine;

namespace BetterVanilla.Components;

public class CosmeticsLoader : MonoBehaviour
{
    private bool IsPostLoadWaitingFinished { get; set; }

    public IEnumerator CoLoadCosmetics()
    {
        Ls.LogInfo($"Waiting for cosmetics bundle versions");
        
        while (FeatureCodeBehaviour.Instance == null || FeatureCodeBehaviour.Instance.Registry == null)
        {
            yield return new WaitForEndOfFrame();
        }
        
        Ls.LogInfo($"Cosmetics bundle versions loaded");

        foreach (var version in FeatureCodeBehaviour.Instance.Registry.CosmeticsBundleVersions)
        {
            Ls.LogInfo($"Loading cosmetics bundle version: {version.Hash}");
            yield return CoLoadBundle(version);
            yield return new WaitForEndOfFrame();
        }

        var hashes = FeatureCodeBehaviour.Instance.Registry.CosmeticsBundleVersions
            .Select(x => x.Hash)
            .ToList();

        foreach (var filePath in Directory.GetFiles(ModPaths.CosmeticsBundlesDirectory))
        {
            var fileName = Path.GetFileName(filePath);
            if (!hashes.Contains(fileName))
            {
                File.Delete(filePath);
            }
        }

        foreach (var filePath in Directory.GetFiles(ModPaths.CosmeticsLocalBundlesDirectory))
        {
            try
            {
                var bundle = CosmeticBundle.FromFile(filePath);
                CosmeticsManager.RegisterBundle(bundle);
            }
            catch (Exception ex)
            {
                Ls.LogError($"Failed to load local cosmetics bundle '{filePath}': {ex.Message}");
            }
        }

        CosmeticsManager.ProcessUnregisteredCosmetics();
    }

    private static IEnumerator CoLoadBundle(CosmeticsBundleVersion version)
    {
        var bundleFilePath = Path.Combine(ModPaths.CosmeticsBundlesDirectory, version.Hash);

        if (!File.Exists(bundleFilePath))
        {
            Ls.LogInfo($"Downloading cosmetics bundle {version.Hash}");
            yield return RequestUtils.CoDownloadFile(version.DownloadUrl, bundleFilePath);
        }

        if (!File.Exists(bundleFilePath))
        {
            Ls.LogError($"Unable to download cosmetics bundle: {bundleFilePath} from {version.DownloadUrl}");
            yield break;
        }

        Ls.LogInfo($"Deserializing cosmetics bundle {version.Hash}");
        var bundle = CosmeticBundle.FromFile(bundleFilePath);

        yield return new WaitForEndOfFrame();

        Ls.LogInfo($"Registering cosmetics bundle {version.Hash}");
        CosmeticsManager.RegisterBundle(bundle);
    }

    private void Update()
    {
        var accountManager = AccountManager.Instance;
        if (accountManager == null) return;
        IsPostLoadWaitingFinished = IsPostLoadWaitingFinished || !accountManager.postLoadWaiting.activeSelf;
        accountManager.postLoadWaiting.SetActive(true);
    }

    private void OnDisable()
    {
        var accountManager = AccountManager.Instance;
        if (accountManager == null) return;
        accountManager.postLoadWaiting.SetActive(!IsPostLoadWaitingFinished);
    }
}