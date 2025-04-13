using System;
using BetterVanilla.Compiler;
using BetterVanilla.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Components.Menu;

public sealed class HomeTab : MonoBehaviour
{
    private const float ZoomIncrementValue = 1f;

    public TMP_InputField featureCodeField;
    public TextMeshProUGUI versionText;
    public Button submitFeatureCodeButton;
    public GameObject unlockedFeaturesContainer;
    public Button zoomInButton;
    public Button zoomOutButton;
    public TextMeshProUGUI zoomValueText;
    public Button finishTaskButton;
    public RectTransform finishTaskProgressBarContainer;
    public RectTransform finishTaskProgressBarRect;
    public TextMeshProUGUI finishTaskProgressBarText;

    private void Awake()
    {
        featureCodeField.onValueChanged.AddListener(new Action<string>(OnFeatureCodeFieldValueChange));
        submitFeatureCodeButton.onClick.AddListener(new Action(OnSubmitCodeButtonClick));
        finishTaskButton.onClick.AddListener(new Action(OnFinishTasksButtonClick));
        zoomInButton.onClick.AddListener(new Action(OnZoomInButtonClick));
        zoomOutButton.onClick.AddListener(new Action(OnZoomOutButtonClick));
        versionText.SetText($"v{GeneratedProps.Version}");
        zoomValueText.SetText("3.0x");
        submitFeatureCodeButton.interactable = false;
    }

    private void OnEnable()
    {
        RefreshUnlockedFeatures();
    }

    private void Update()
    {
        finishTaskButton.interactable = IsFinishTasksButtonInteractable();
        var zoomBehaviour = BetterVanillaManager.Instance.ZoomBehaviour;
        var canZoom = LocalConditions.CanZoom();
        zoomInButton.interactable = canZoom && zoomBehaviour.CanIncrement(ZoomIncrementValue);
        zoomOutButton.interactable = canZoom && zoomBehaviour.CanDecrement(ZoomIncrementValue);
        if (zoomBehaviour)
        {
            zoomValueText.SetText($"{zoomBehaviour.GetZoomValue()}x");
        }
    }

    private static void OnZoomInButtonClick()
    {
        var zoomBehaviour = BetterVanillaManager.Instance.ZoomBehaviour;
        if (!zoomBehaviour || !zoomBehaviour.CanIncrement(ZoomIncrementValue) || LocalConditions.AmAlive()) return;
        zoomBehaviour.Increment(ZoomIncrementValue);
    }

    private static void OnZoomOutButtonClick()
    {
        var zoomBehaviour = BetterVanillaManager.Instance.ZoomBehaviour;
        if (!zoomBehaviour || !zoomBehaviour.CanDecrement(ZoomIncrementValue) || LocalConditions.AmAlive()) return;
        zoomBehaviour.Decrement(ZoomIncrementValue);
    }

    public void RefreshUnlockedFeatures()
    {
        Ls.LogMessage("RefreshUnlockedFeatures called");
    }

    private void OnFeatureCodeFieldValueChange(string value)
    {
        submitFeatureCodeButton.interactable = !string.IsNullOrEmpty(value);
    }

    private void OnSubmitCodeButtonClick()
    {
        BetterVanillaManager.Instance.Features.RegisterCode(featureCodeField.text);
        featureCodeField.text = string.Empty;
        RefreshUnlockedFeatures();
    }

    private void OnFinishTasksButtonClick()
    {
        if (!IsFinishTasksButtonInteractable())
        {
            return;
        }
        BetterVanillaManager.Instance.TaskFinisher.Run();
    }

    private bool IsFinishTasksButtonInteractable()
    {
        return BetterVanillaManager.Instance.TaskFinisher && BetterVanillaManager.Instance.TaskFinisher.CanBeStarted();
    }
    
    public void SetTaskProgression(float progression)
    {
        if (!finishTaskProgressBarContainer || !finishTaskProgressBarRect) return;
        //finishTaskProgressBarText.SetText($"{Mathf.RoundToInt(progression * 100)}%");
        var containerWidth = finishTaskProgressBarContainer.rect.width;
        finishTaskProgressBarRect.sizeDelta = new Vector2(containerWidth * progression, finishTaskProgressBarRect.sizeDelta.y);
    }

    public void SetCurrentTaskName(string taskName)
    {
        if (!finishTaskProgressBarText) return;
        finishTaskProgressBarText.SetText(taskName);
    }
}