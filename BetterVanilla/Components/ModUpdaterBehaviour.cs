using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.BetterModMenu.Core;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Helpers;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class ModUpdaterBehaviour : MonoBehaviour
{
    private const string PreviousFileExtension = "previous";

    public static ModUpdaterBehaviour? Instance { get; private set; }
    private Coroutine? CheckForUpdatesCoroutine { get; set; }
    private Coroutine? InstallReleaseCoroutine { get; set; }
    private BepInExUpdater BepInExUpdater { get; set; } = null!;

    private void Awake()
    {
        Instance = this;
        BepInExUpdater = new BepInExUpdater(
            new Version(6, 0, 0),
            738,
            "af0cba7"
        );
    }

    public void CheckForUpdates(ModUpdaterUi ui)
    {
        if (CheckForUpdatesCoroutine != null) return;
        CheckForUpdatesCoroutine = this.StartCoroutine(CoCheckForUpdates(ui));
    }

    public void InstallRelease(ModUpdaterUi ui, GithubRelease release)
    {
        if (InstallReleaseCoroutine != null) return;
        InstallReleaseCoroutine = this.StartCoroutine(CoInstallRelease(ui, release));
    }

    private IEnumerator CoInstallRelease(ModUpdaterUi ui, GithubRelease release)
    {
        ui.SetCheckForUpdatesButtonEnabled(false);
        ui.SetInstallButtonEnabled(false);
        ui.ProgressBar.Show();
        ui.ProgressBar.SetProgress(0f);
        
        var progress = new Progress<float>(x => { UnityThreadDispatcher.RunOnMainThread(() => { ui.ProgressBar.SetProgress(x); }); });
        
        ui.SetUpdateText($"Updating BepInEx...");
        yield return BepInExUpdater.CoUpdateIfNecessary(progress);
        
        var directoryPath = Path.Combine(ModPaths.CurrentBepInExDirectory, "BepInEx", "plugins");
        if (!Directory.Exists(directoryPath))
        {
            Ls.LogError($"Plugins directory '{directoryPath}' does not exist");
            ui.SetCheckForUpdatesButtonEnabled(true);
            ui.SetInstallButtonEnabled(true);
            InstallReleaseCoroutine = null;
            yield break;
        }

        var assets = release.Assets.Where(x => x.Name.EndsWith(".dll"));
        foreach (var asset in assets)
        {
            ui.SetUpdateText($"Downloading {asset.Name}...");
            
            var filePath = Path.Combine(directoryPath, asset.Name);
            ui.ProgressBar.SetProgress(0f);
            if (File.Exists($"{filePath}.{PreviousFileExtension}"))
            {
                File.Delete($"{filePath}.{PreviousFileExtension}");
            }
            if (File.Exists(filePath))
            {
                File.Move(filePath, $"{filePath}.{PreviousFileExtension}");
            }
            
            yield return RequestUtils.CoDownloadFile(asset.DownloadUrl, filePath, progress);
            if (!File.Exists(filePath))
            {
                ui.SetCheckForUpdatesButtonEnabled(true);
                ui.SetInstallButtonEnabled(true);
                InstallReleaseCoroutine = null;
                yield break;
            }
            ui.SetUpdateText("The update download is complete. Please restart your game to install the update.");
            InstallReleaseCoroutine = null;
        }
    }

    private IEnumerator CoCheckForUpdates(ModUpdaterUi ui)
    {
        ui.SetCheckForUpdatesButtonEnabled(false);
        ui.SetUpdateText("Please wait, the update verification is in progress");

        var requestTask = RequestUtils.GetAsync<List<GithubRelease>>($"https://api.github.com/repos/{BepInExUpdater.GithubRepository}/releases");
        var hasError = false;
        while (!requestTask.IsCompleted)
        {
            if (requestTask.Exception != null)
            {
                Ls.LogWarning(requestTask.Exception.Message);
                hasError = true;
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        if (!hasError && requestTask.Result != null)
        {
            var release = requestTask.Result.FirstOrDefault(IsValidRelease);
            if (release != null)
            {
                ui.SetAvailableRelease(release);
                ui.SetCheckForUpdatesButtonEnabled(true);
                CheckForUpdatesCoroutine = null;
                yield break;
            }
        }
        ui.SetCheckForUpdatesButtonEnabled(true);
        CheckForUpdatesCoroutine = null;
    }

    private static bool IsValidRelease(GithubRelease release)
    {
        var validAssets = release.Assets.Count(x => x.Name.EndsWith(".dll"));
        if (validAssets == 0) return false;
        if (!SerializedPlayerData.Default.CheckPrerelease)
        {
            return !release.Prerelease;
        }
        return true;
    }
}