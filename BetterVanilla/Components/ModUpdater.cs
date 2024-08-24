using System;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components.BaseComponents;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using BetterVanilla.Compiler;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BetterVanilla.Components;

public sealed class ModUpdater : BaseCloseableUiComponent
{
    private const string PreviousFileExtension = "previous";
    
    public TextMeshProUGUI updaterText;
    public Button installButton;
    public RectTransform progressBarContainer;
    public RectTransform progressBarRect;
    public TextMeshProUGUI progressBarText;
    
    private GithubRelease LatestRelease { get; set; }
    private HashSet<string> AllowedDllNames { get; set; }

    protected override void Awake()
    {
        base.Awake();
        installButton.onClick.AddListener(new Action(OnInstallButtonClick));
        Close();
    }

    public void Initialize(string githubRepository, params string[] dllNames)
    {
        AllowedDllNames = dllNames.ToHashSet();
        this.StartCoroutine(CoCheckForUpdates(githubRepository));
    }

    private void OnInstallButtonClick()
    {
        this.StartCoroutine(CoDownloadLatestRelease());
    }

    private IEnumerator CoCheckForUpdates(string githubRepository)
    {
        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        www.SetUrl($"https://api.github.com/repos/{githubRepository}/releases/latest");
        www.downloadHandler = new DownloadHandlerBuffer();
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            yield return null;
        }
        if (www.isNetworkError || www.isHttpError)
        {
            Ls.LogError(www.error);
            yield break;
        }
        LatestRelease = JsonSerializer.Deserialize<GithubRelease>(www.downloadHandler.text);
        www.downloadHandler.Dispose();
        www.Dispose();

        if (LatestRelease == null)
        {
            Ls.LogError("No release found");
            yield break;
        }

        if (GeneratedProps.Version == LatestRelease.Version.ToString())
        {
            yield break;
        }
        
        Open();
    }

    private IEnumerator CoDownloadLatestRelease()
    {
        SetUiState(true);

        var filePath = Assembly.GetExecutingAssembly().Location;
        var fileName = Path.GetFileName(filePath);
        var directoryPath = Path.GetDirectoryName(filePath);

        if (directoryPath == null)
        {
            Ls.LogError($"Unable to find directory of {filePath}");
            SetUiState(false);
            yield break;
        }

        var asset = LatestRelease?.Assets.Find(x => AllowedDllNames.Contains(x.Name));
        if (asset == null)
        {
            Ls.LogWarning($"Unable to find {fileName} in release assets");
            SetUiState(false);
            yield break;
        }
        
        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        www.SetUrl(asset.DownloadUrl);
        www.downloadHandler = new DownloadHandlerBuffer();
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            SetProgression(operation.progress);
            yield return new WaitForEndOfFrame();
        }
        SetProgression(1f);
        if (File.Exists($"{filePath}.{PreviousFileExtension}"))
        {
            File.Delete($"{filePath}.{PreviousFileExtension}");
        }
        if (File.Exists(filePath))
        {
            File.Move(filePath, $"{filePath}.{PreviousFileExtension}");
        }

        var assetFileName = Path.GetFileName(asset.DownloadUrl);
        if (assetFileName == null)
        {
            Ls.LogError($"Unable to find file name of {asset.DownloadUrl}");
            SetUiState(false);
            yield break;
        }
        
        var saveFileTask = File.WriteAllBytesAsync(Path.Combine(directoryPath, assetFileName), www.downloadHandler.data);
        var hasError = false;
        while (!saveFileTask.IsCompleted)
        {
            if (saveFileTask.Exception != null)
            {
                Ls.LogWarning(saveFileTask.Exception.Message);
                hasError = true;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        
        www.downloadHandler.Dispose();
        www.Dispose();

        if (!hasError)
        {
            closeButton.interactable = true;
            SetUpdaterText("The update was successfully completed.\nPlease restart the game to launch the new version.");
            yield break;
        }
        SetUiState(false);
    }

    private void SetUiState(bool installing)
    {
        closeButton.interactable = !installing;
        SetProgression(0f);
        SetInstallButtonActive(!installing);
        SetProgressBarActive(installing);
    }

    private void SetUpdaterText(string text)
    {
        updaterText.SetText(text);
    }

    private void SetInstallButtonActive(bool state)
    {
        installButton.gameObject.SetActive(state);
    }

    private void SetProgressBarActive(bool state)
    {
        progressBarContainer.gameObject.SetActive(state);
    }

    private void SetProgression(float progression)
    {
        progressBarText.SetText($"{Mathf.RoundToInt(progression * 100)}%");
        var containerWidth = progressBarContainer.rect.width;
        progressBarRect.sizeDelta = new Vector2(containerWidth * progression, progressBarRect.sizeDelta.y);
    }
}