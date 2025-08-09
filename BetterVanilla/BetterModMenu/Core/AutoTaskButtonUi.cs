using System;
using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.BetterModMenu.Core;

public sealed class AutoTaskButtonUi : MonoBehaviour
{
    public Button button = null!;
    public ProgressBarUi progressBar = null!;
    public Image progressBarBackground = null!;
    public Color notStartedColor;
    public Color cancelColor;
    public Color runningColor;
    public Color pausedColor;
    public Color completedColor;

    private void Awake()
    {
        GameEventManager.GameStarted += Reset;
    }

    public void OnAutoTaskButtonClicked()
    {
        if (AutoTaskBehaviour.Instance == null) return;
        if (AutoTaskBehaviour.Instance.IsStarted)
        {
            Ls.LogMessage($"Stopping AutoTaskBehaviour");
            AutoTaskBehaviour.Instance.StateUpdated -= OnAutoTaskBehaviourStateUpdated;
            AutoTaskBehaviour.Instance.Stop();
            Reset();
        }
        else
        {
            Ls.LogMessage($"Starting AutoTaskBehaviour");
            AutoTaskBehaviour.Instance.StateUpdated += OnAutoTaskBehaviourStateUpdated;
            var progress = new Progress<float>(progressBar.SetProgress);
            AutoTaskBehaviour.Instance.Run(progress);
        }
    }

    public void Reset()
    {
        button.image.color = notStartedColor;
        button.interactable = true;
        progressBar.SetProgress(0f);
    }

    private void OnAutoTaskBehaviourStateUpdated(AutoTaskState previousState, AutoTaskState currentState)
    {
        switch (currentState)
        {
            case AutoTaskState.Running:
                button.image.color = cancelColor;
                progressBarBackground.color = runningColor;
                button.interactable = true;
                break;
            case AutoTaskState.Paused:
                button.image.color = cancelColor;
                button.interactable = true;
                progressBarBackground.color = pausedColor;
                break;
            case AutoTaskState.Completed:
                button.image.color = cancelColor;
                button.interactable = false;
                progressBarBackground.color = completedColor;
                break;
        }
        if (currentState == AutoTaskState.Running)
        {
            
            return;
        }
        if (currentState == AutoTaskState.Paused)
        {
            
            return;
        }
        if (currentState == AutoTaskState.Completed)
        {
            progressBarBackground.color = completedColor;
            button.interactable = false;
        }
    }
}