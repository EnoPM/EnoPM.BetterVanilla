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

    private string GithubRepository { get; set; } = null!;
    private string ModFilePath { get; set; } = null!;
    private Coroutine? CheckForUpdatesCoroutine { get; set; }
    private Coroutine? InstallReleaseCoroutine { get; set; }

    private void Awake()
    {
        Instance = this;
        GithubRepository = "EnoPM/EnoPM.BetterVanilla";
        ModFilePath = typeof(BetterVanillaPlugin).Assembly.Location;
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

        var directoryPath = Path.GetDirectoryName(ModFilePath);
        if (directoryPath == null)
        {
            Ls.LogError($"Unable to find directory of {ModFilePath}");
            ui.SetCheckForUpdatesButtonEnabled(true);
            ui.SetInstallButtonEnabled(true);
            InstallReleaseCoroutine = null;
            yield break;
        }

        var assets = release.Assets.Where(x => x.Name.EndsWith(".dll"));
        foreach (var asset in assets)
        {
            ui.ProgressBar.SetProgress(0f);
            if (File.Exists($"{ModFilePath}.{PreviousFileExtension}"))
            {
                File.Delete($"{ModFilePath}.{PreviousFileExtension}");
            }
            File.Move(ModFilePath, $"{ModFilePath}.{PreviousFileExtension}");

            var destinationPath = Path.Combine(directoryPath, asset.Name);

            var progress = new Progress<float>(x => { UnityThreadDispatcher.RunOnMainThread(() => { ui.ProgressBar.SetProgress(x); }); });
            var requestTask = RequestUtils.DownloadFileAsync(asset.DownloadUrl, destinationPath, progress);

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

            if (!hasError)
            {
                ui.SetUpdateText("The update download is complete. Please restart your game to install the update.");
                InstallReleaseCoroutine = null;
                yield break;
            }
            ui.SetCheckForUpdatesButtonEnabled(true);
            ui.SetInstallButtonEnabled(true);
            InstallReleaseCoroutine = null;
        }
    }

    private IEnumerator CoCheckForUpdates(ModUpdaterUi ui)
    {
        ui.SetCheckForUpdatesButtonEnabled(false);
        ui.SetUpdateText("Please wait, the update verification is in progress");

        var requestTask = RequestUtils.GetAsync<List<GithubRelease>>($"https://api.github.com/repos/{GithubRepository}/releases");
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

    private bool IsValidRelease(GithubRelease release)
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