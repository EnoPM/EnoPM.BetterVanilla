using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.Json;
using BepInEx.Unity.IL2CPP.Utils;
using EnoPM.BetterVanilla.Api;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Patches;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public class ModUpdaterController : MonoBehaviour
{
    private const string GithubRepository = "EnoPM/EnoPM.BetterVanilla";
    private const string PreviousFileExtension = "previous";
    
    public static ModUpdaterController Instance { get; private set; }
    
    public GameObject canvas;
    public Button closeButton;
    public Button installButton;
    public TextMeshProUGUI updateText;
    public RectTransform progressBarContainerRect;
    public RectTransform progressBarRect;
    public TextMeshProUGUI progressBarText;
    
    private GithubRelease LatestRelease { get; set; }
    private readonly PassiveButtonsBlocker _overlayBlocker = new();

    private void Awake()
    {
        Instance = this;
        canvas.SetActive(false);
        closeButton.onClick.AddListener((UnityAction)Close);
        installButton.onClick.AddListener((UnityAction)DownloadLatestRelease);
        SetProgressionActive(false);
        SetInstallButtonActive(true);
    }

    private void Start()
    {
        Plugin.Logger.LogMessage($"{nameof(ModUpdaterController)} started");
        this.StartCoroutine(CoCheckForUpdate());
    }

    private IEnumerator CoCheckForUpdate()
    {
        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        www.SetUrl($"https://api.github.com/repos/{GithubRepository}/releases/latest");
        www.downloadHandler = new DownloadHandlerBuffer();
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        if (www.isNetworkError || www.isHttpError)
        {
            Plugin.Logger.LogWarning(www.error);
            yield break;
        }

        LatestRelease = JsonSerializer.Deserialize<GithubRelease>(www.downloadHandler.text);
        www.downloadHandler.Dispose();
        www.Dispose();

        if (LatestRelease == null)
        {
            Plugin.Logger.LogWarning("No release found");
            yield break;
        }
        
        Plugin.Logger.LogMessage($"Latest version: {LatestRelease.Version.ToString()}");
        Plugin.Logger.LogMessage($"Current version: {Version.Parse(PluginProps.Version).ToString()}");

        if (!LatestRelease.IsNewer(Version.Parse(PluginProps.Version)))
        {
            yield break;
        }
        
        Open();
    }
    
    private void SetUiState(bool installing)
    {
        closeButton.interactable = !installing;
        SetProgression(0f);
        SetInstallButtonActive(!installing);
        SetProgressionActive(installing);
    }
    
    private void DownloadLatestRelease()
    {
        this.StartCoroutine(CoDownloadLatestRelease());
    }

    private IEnumerator CoDownloadLatestRelease()
    {
        SetUiState(true);

        var filePath = Assembly.GetExecutingAssembly().Location;
        var fileName = Path.GetFileName(filePath);

        var asset = LatestRelease?.Assets.Find(x => x.Name == fileName);
        if (asset == null)
        {
            Plugin.Logger.LogWarning($"Unable to find {fileName} in release assets");
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
        
        var saveFileTask = File.WriteAllBytesAsync(filePath, www.downloadHandler.data);
        var hasError = false;
        while (!saveFileTask.IsCompleted)
        {
            if (saveFileTask.Exception != null)
            {
                Plugin.Logger.LogWarning(saveFileTask.Exception.Message);
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
            SetUpdateText("The update was successfully completed.\nPlease restart the game to launch the new version.");
            yield break;
        }
        SetUiState(false);
    }

    public void Open()
    {
        canvas.SetActive(true);
        _overlayBlocker.Block();
    }

    public void Close()
    {
        canvas.SetActive(false);
        _overlayBlocker.Unblock();
    }

    public void SetUpdateText(string text)
    {
        updateText.SetText(text);
    }

    public void SetInstallButtonActive(bool active)
    {
        installButton.gameObject.SetActive(active);
    }

    public void SetProgressionActive(bool active)
    {
        progressBarContainerRect.gameObject.SetActive(active);
    }

    public void SetProgression(float progression)
    {
        progressBarText.SetText($"{Mathf.RoundToInt(progression * 100)}%");
        var containerWidth = progressBarContainerRect.rect.width;
        progressBarRect.sizeDelta = new Vector2((containerWidth - 4f) * progression, progressBarRect.sizeDelta.y);
    }
}