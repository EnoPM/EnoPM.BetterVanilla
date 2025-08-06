using System;
using BetterVanilla.Components;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Helpers;
using BetterVanilla.GeneratedRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.BetterModMenu.Core;

public sealed class ModUpdaterUi : MonoBehaviour
{
    public Button checkForUpdatesButton = null!;
    public TextMeshProUGUI checkForUpdatesButtonText = null!;
    public Button installButton = null!;
    public TextMeshProUGUI installButtonText = null!;
    public TextMeshProUGUI updateText = null!;
    public ProgressBarUi progressBar = null!;
    
    public ProgressBarUi ProgressBar => progressBar;
    
    private GithubRelease? AvailableRelease { get; set; }

    private void Start()
    {
        progressBar.Hide();
    }

    public void SetUpdateText(string text)
    {
        if (!updateText) return;
        updateText.SetText(text);
    }

    public void SetCheckForUpdatesButtonEnabled(bool isEnabled)
    {
        checkForUpdatesButton.interactable = isEnabled;
    }

    public void SetInstallButtonEnabled(bool isEnabled)
    {
        installButton.interactable = isEnabled;
    }

    public void SetAvailableRelease(GithubRelease release)
    {
        AvailableRelease = release;

        if (AvailableRelease.Version.ToString() == GeneratedProps.Version)
        {
            SetUpdateText("No update available. You are using the latest version of BetterVanilla");
            installButtonText.SetText("No update to install");
        }
        else
        {
            var currentVersion = ColorUtils.ColoredString(ColorUtils.FromHex("#A01919"), $"v{GeneratedProps.Version}");
            var releaseVersion = ColorUtils.ColoredString(ColorUtils.FromHex("#00B03A"), $"v{AvailableRelease.Version.ToString()}");
            SetUpdateText($"BetterVanilla {releaseVersion} is available. You are currently using BetterVanilla {currentVersion}");
            installButtonText.SetText($"Install BetterVanilla v{AvailableRelease.Version.ToString()}");
            SetInstallButtonEnabled(true);
        }
    }

    public void OnCheckForUpdatesButtonClicked()
    {
        if (ModUpdaterBehaviour.Instance == null) return;
        ModUpdaterBehaviour.Instance.CheckForUpdates(this);
    }

    public void OnInstallButtonClicked()
    {
        if (ModUpdaterBehaviour.Instance == null || AvailableRelease == null) return;
        ModUpdaterBehaviour.Instance.InstallRelease(this, AvailableRelease);
    }
}