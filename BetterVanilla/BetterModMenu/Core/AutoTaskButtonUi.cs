using System.Collections;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
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
    
    private Coroutine? AutoTaskCoroutine { get; set; }
    private Vector2? CurrentPosition { get; set; }
    
    public bool IsRunning => AutoTaskCoroutine != null;

    private void Awake()
    {
        GameEventManager.GameStarted += Reset;
    }

    public void OnAutoTaskButtonClicked()
    {
        if (AutoTaskCoroutine != null)
        {
            Ls.LogMessage($"Stopping AutoTaskBehaviour");
            StopCoroutine(AutoTaskCoroutine);
            AutoTaskCoroutine = null;
            Reset();
        }
        else
        {
            Ls.LogMessage($"Starting AutoTaskBehaviour");
            AutoTaskCoroutine = this.StartCoroutine(CoRun());
        }
    }

    private IEnumerator CoRun()
    {
        CurrentPosition = null;
        var remainingTasks = PlayerControl.LocalPlayer.GetRemainingTasks();
        while (remainingTasks.Count > 0 && LocalConditions.CanCompleteAutoTasks())
        {
            if (MeetingHud.Instance != null)
            {
                SetPaused();
                yield return new WaitForEndOfFrame();
                continue;
            }
            SetRunning();
            var taskToComplete = remainingTasks.PickOneRandom();
            yield return CoCompleteTask(taskToComplete);
            remainingTasks = PlayerControl.LocalPlayer != null ? PlayerControl.LocalPlayer.GetRemainingTasks() : [];
        }
        SetCompleted();
        progressBar.SetProgress(1f);
        AutoTaskCoroutine = null;
    }
    
    private IEnumerator CoCompleteTask(NormalPlayerTask task)
    {
        Ls.LogMessage($"Autocompleting task '{TranslationController.Instance.GetString(TranslationController.Instance.GetTaskName(task.TaskType))}'");
        progressBar.SetProgress(0f);
        const float duration = 5f;
        var taskPosition = GetTaskConsolePosition(task);
        var currentPosition = GetCurrentPosition();
        var speed = GetPlayerSpeed();
        var travelTime = speed > 0f ? Vector2.Distance(currentPosition, taskPosition) / speed : 10f;
        var taskDuration = travelTime + duration;
        var timer = 0f;
        while (travelTime > timer)
        {
            if (MeetingHud.Instance != null)
            {
                yield break;
            }
            timer += Time.deltaTime;
            progressBar.SetProgress(timer / taskDuration);
            yield return new WaitForEndOfFrame();
        }
        CurrentPosition = taskPosition;
        
        while (taskDuration > timer)
        {
            if (MeetingHud.Instance != null)
            {
                yield break;
            }
            timer += Time.deltaTime;
            progressBar.SetProgress(timer / taskDuration);
            yield return new WaitForEndOfFrame();
        }

        if (MeetingHud.Instance != null)
        {
            yield break;
        }
        
        progressBar.SetProgress(1f);
        task.taskStep = task.MaxStep;
        task.NextStep();
        yield return new WaitForSeconds(1f);
    }
    
    private Vector2 GetCurrentPosition()
    {
        if (CurrentPosition != null) return CurrentPosition.Value;
        return PlayerControl.LocalPlayer.transform.position;
    }

    private static float GetPlayerSpeed()
    {
        return PlayerControl.LocalPlayer.MyPhysics.TrueSpeed;
    }

    private static Vector2 GetTaskConsolePosition(NormalPlayerTask task)
    {
        var consolePositions = task.FindConsolesPos();
        return consolePositions._items.FirstOrDefault();
    }

    public void Reset()
    {
        button.image.color = notStartedColor;
        button.interactable = true;
        progressBar.SetProgress(0f);
    }

    private void SetRunning()
    {
        button.image.color = cancelColor;
        progressBarBackground.color = runningColor;
        button.interactable = true;
    }

    private void SetPaused()
    {
        button.image.color = cancelColor;
        button.interactable = true;
        progressBarBackground.color = pausedColor;
        CurrentPosition = null;
    }

    private void SetCompleted()
    {
        button.image.color = cancelColor;
        button.interactable = false;
        progressBarBackground.color = completedColor;
    }
}