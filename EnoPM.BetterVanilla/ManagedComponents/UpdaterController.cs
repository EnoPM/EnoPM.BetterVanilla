using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using BepInEx.Unity.IL2CPP.Utils;
using EnoPM.BetterVanilla.Api;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.ManagedComponents;

public class UpdaterController : MonoBehaviour
{
    private const string GithubRepository = "EnoPM/EnoPM.BetterVanilla";
    
    public GameObject canvas;
    public GameObject updaterPanel;
    public TextMeshProUGUI titleText;
    public Button closeButton;
    public Image icon;
    public Button checkForUpdatesButton;
    public Button installButton;
    public TextMeshProUGUI installButtonText;
    public TextMeshProUGUI selectVersionText;
    public TMP_Dropdown versionSelectorDropdown;
    public GameObject progressBarContainer;
    public RectTransform progressBarContainerRect;
    public RectTransform progressBarRect;
    public TextMeshProUGUI changelogTitleText;
    public TextMeshProUGUI changelogText;
    
    private List<GithubRelease> AvailableReleases { get; set; }
    private GithubRelease SelectedRelease { get; set; }

    private void Start()
    {
        Close();
        installButton.interactable = false;
        versionSelectorDropdown.interactable = false;
        closeButton.onClick.AddListener((UnityAction)Close);
        checkForUpdatesButton.onClick.AddListener((UnityAction)CheckForUpdates);
        installButton.onClick.AddListener((UnityAction)OnInstallButtonClick);
        versionSelectorDropdown.onValueChanged.AddListener((UnityAction<int>)OnSelectVersion);
        CheckForUpdates();
    }
    
    private IEnumerator CoShowError(string errorMessage)
    {
        Plugin.Logger.LogError(errorMessage);
        yield return Plugin.ErrorPopup.CoShow("Updater Error",errorMessage);
    }
    
    private IEnumerator CoShowSuccess()
    {
        Plugin.Logger.LogMessage("Update successfully installed.");
        yield return Plugin.SuccessPopup.CoShow("Update installed", "The update has been installed successfully. Please restart your game in order to play on the latest version.");
    }

    private void SetProgressBarActive(bool active) => progressBarContainer.SetActive(active);
    private void SetProgressBarProgression(float progression) => progressBarRect.sizeDelta = new Vector2((progressBarContainerRect.sizeDelta.x - 4f) * progression, progressBarRect.sizeDelta.y);

    private void OnSelectVersion(int index)
    {
        SelectedRelease = AvailableReleases[index];
        changelogTitleText.SetText($"What's new in {SelectedRelease.Tag}");
        changelogText.SetText(SelectedRelease.Description);
    }

    private void CheckForUpdates()
    {
        this.StartCoroutine(CoCheckForUpdates());
    }
    
    private IEnumerator CoCheckForUpdates()
    {
        SetProgressBarActive(false);
        checkForUpdatesButton.interactable = false;
        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        www.SetUrl($"https://api.github.com/repos/{GithubRepository}/releases");
        www.downloadHandler = new DownloadHandlerBuffer();
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        if (www.isNetworkError || www.isHttpError)
        {
            yield return CoShowError(www.error);
            checkForUpdatesButton.interactable = true;
            yield break;
        }

        AvailableReleases = JsonSerializer.Deserialize<List<GithubRelease>>(www.downloadHandler.text);
        www.downloadHandler.Dispose();
        www.Dispose();

        if (AvailableReleases.Count == 0)
        {
            yield return CoShowError("No release found");
            checkForUpdatesButton.interactable = true;
            yield break;
        }

        var options = new Il2CppSystem.Collections.Generic.List<string>();
        foreach (var release in AvailableReleases)
        {
            options.Add(release.Tag);
        }

        versionSelectorDropdown.ClearOptions();
        versionSelectorDropdown.AddOptions(options);

        var latestRelease = AvailableReleases.First();
        OnSelectVersion(0);

        checkForUpdatesButton.interactable = true;
        installButton.interactable = true;
        versionSelectorDropdown.interactable = true;

        if (latestRelease.IsNewer(Version.Parse(PluginProps.Version)))
        {
            Open();
        }
    }
    
    private IEnumerator CoDownloadRelease()
    {
        installButton.interactable = false;
        versionSelectorDropdown.interactable = false;
        checkForUpdatesButton.interactable = false;

        var filePath = Assembly.GetExecutingAssembly().Location;
        var fileName = Path.GetFileName(filePath);
        Plugin.Logger.LogMessage(filePath);
        Plugin.Logger.LogMessage(fileName);
        var asset = SelectedRelease.Assets.Find(x => x.Name == fileName);
        if (asset == null)
        {
            yield return CoShowError($"Unable to find {fileName} file in release");
            installButton.interactable = true;
            versionSelectorDropdown.interactable = true;
            checkForUpdatesButton.interactable = true;
            yield break;
        }

        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        www.SetUrl(asset.DownloadUrl);
        www.downloadHandler = new DownloadHandlerBuffer();
        var operation = www.SendWebRequest();

        SetProgressBarActive(true);
        while (!operation.isDone)
        {
            SetProgressBarProgression(operation.progress);
            yield return new WaitForEndOfFrame();
        }
        SetProgressBarProgression(1f);
        if (File.Exists($"{filePath}.old"))
        {
            File.Delete($"{filePath}.old");
        }
        if (File.Exists(filePath))
        {
            File.Move(filePath, $"{filePath}.old");
        }

        var saveFileTask = File.WriteAllBytesAsync(filePath, www.downloadHandler.data);
        var hasError = false;
        while (!saveFileTask.IsCompleted)
        {
            if (saveFileTask.Exception != null)
            {
                yield return CoShowError(saveFileTask.Exception.Message);
                hasError = true;
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        www.downloadHandler.Dispose();
        www.Dispose();
        SetProgressBarActive(false);

        if (!hasError)
        {
            yield return CoShowSuccess();
        }
        else
        {
            installButton.interactable = true;
            versionSelectorDropdown.interactable = true;
            checkForUpdatesButton.interactable = true;
        }
    }

    private void OnInstallButtonClick()
    {
        this.StartCoroutine(CoDownloadRelease());
    }

    public void Close() => canvas.SetActive(false);

    public void Open() => canvas.SetActive(true);
}