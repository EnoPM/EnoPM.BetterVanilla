using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Compiler;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Core.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Components.Menu;

public sealed class HomeTab : MonoBehaviour
{
    private const string FinishAllMyTasksFeatureHash = "1C27A7F613EA4EFB37B3A18F8B2DC40E6167EFFFD34D1FA139199199C427860F";
    private const float ZoomIncrementValue = 1f;

    public TMP_InputField featureCodeField;
    public TextMeshProUGUI versionText;
    public Button submitFeatureCodeButton;
    public GameObject unlockedFeaturesContainer;
    public Button zoomInButton;
    public Button zoomOutButton;
    public TextMeshProUGUI zoomValueText;
    public Button finishTaskButton;

    private Coroutine FinishTasksRoutine { get; set; }

    private void Awake()
    {
        featureCodeField.onValueChanged.AddListener(new Action<string>(OnFeatureCodeFieldValueChange));
        submitFeatureCodeButton.onClick.AddListener(new Action(OnSubmitCodeButtonClick));
        finishTaskButton.onClick.AddListener(new Action(OnFinishTasksButtonClick));
        zoomInButton.onClick.AddListener(new Action(OnZoomInButtonClick));
        zoomOutButton.onClick.AddListener(new Action(OnZoomOutButtonClick));
        versionText.SetText($"v{GeneratedProps.Version}");
        zoomValueText.SetText("3.0x");
        BetterVanillaManager.Instance.Features.RegisterHash(FinishAllMyTasksFeatureHash);
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
        var canZoom = zoomBehaviour && ConditionUtils.AmDead();
        zoomInButton.interactable = canZoom && zoomBehaviour.CanIncrement(ZoomIncrementValue);
        zoomOutButton.interactable = canZoom && zoomBehaviour.CanDecrement(ZoomIncrementValue);
        if (zoomBehaviour)
        {
            zoomValueText.SetText($"{zoomBehaviour.GetZoomValue()}x");
        }
    }

    private static void OnZoomInButtonClick()
    {
        if (!BetterVanillaManager.Instance.ZoomBehaviour || !BetterVanillaManager.Instance.ZoomBehaviour.CanIncrement(ZoomIncrementValue) || ConditionUtils.AmAlive()) return;
        BetterVanillaManager.Instance.ZoomBehaviour.Increment(ZoomIncrementValue);
    }

    private static void OnZoomOutButtonClick()
    {
        if (!BetterVanillaManager.Instance.ZoomBehaviour || !BetterVanillaManager.Instance.ZoomBehaviour.CanDecrement(ZoomIncrementValue) || ConditionUtils.AmAlive()) return;
        BetterVanillaManager.Instance.ZoomBehaviour.Decrement(ZoomIncrementValue);
    }

    public void RefreshUnlockedFeatures()
    {
        var isFinishAllMyTasksFeatureUnlocked = BetterVanillaManager.Instance.Features.IsUnlocked(FinishAllMyTasksFeatureHash);
        finishTaskButton.transform.parent.gameObject.SetActive(isFinishAllMyTasksFeatureUnlocked);
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
        if (BetterVanillaManager.Instance.Features.IsLocked(FinishAllMyTasksFeatureHash) || !IsFinishTasksButtonInteractable())
        {
            return;
        }
        FinishTasksRoutine = BetterVanillaManager.Instance.StartCoroutine(CoFinishAllMyTasks());
    }

    private IEnumerator CoFinishAllMyTasks()
    {
        yield return CoroutineUtils.RandomWait();
        while (MeetingHud.Instance)
        {
            yield return CoroutineUtils.RandomWait();
        }
        var remainingTasks = PlayerControl.LocalPlayer.GetRemainingTasks();
        while (remainingTasks.Count > 0 &&  PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.CanFinishTask())
        {
            var task = remainingTasks.GetOneRandom();
            PlayerControl.LocalPlayer.BetterCompleteTask(task);
            yield return CoroutineUtils.RandomWait();
            while (MeetingHud.Instance)
            {
                yield return CoroutineUtils.RandomWait();
            }
            remainingTasks = PlayerControl.LocalPlayer ? PlayerControl.LocalPlayer.GetRemainingTasks() : [];
        }

        FinishTasksRoutine = null;
    }

    private bool IsFinishTasksButtonInteractable()
    {
        return FinishTasksRoutine == null && PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.CanFinishTask();
    }
}