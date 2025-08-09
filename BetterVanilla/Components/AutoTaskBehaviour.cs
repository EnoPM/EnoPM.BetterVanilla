using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class AutoTaskBehaviour : MonoBehaviour
{
    public static AutoTaskBehaviour? Instance { get; private set; }

    private Coroutine? AutoTaskCoroutine { get; set; }
    private Vector2? CurrentPosition { get; set; }
    private AutoTaskState PreviousState { get; set; } = AutoTaskState.Idle;
    private AutoTaskState CurrentState { get; set; } = AutoTaskState.Idle;
    public delegate void AutoTaskStateUpdated(AutoTaskState previousState, AutoTaskState currentState);
    public event AutoTaskStateUpdated? StateUpdated;
    
    public bool IsStarted => AutoTaskCoroutine != null;
    
    private void Awake()
    {
        if (Instance != null)
        {
            throw new Exception($"{nameof(AutoTaskBehaviour)} must be a singleton");
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void SetState(AutoTaskState state)
    {
        if (CurrentState == state) return;
        PreviousState = CurrentState;
        CurrentState = state;
        StateUpdated?.Invoke(PreviousState, CurrentState);
        if (CurrentState != AutoTaskState.Running)
        {
            CurrentPosition = null;
        }
    }

    public void Run(Progress<float> progress)
    {
        if (IsStarted) return;
        AutoTaskCoroutine = this.StartCoroutine(CoRun(progress));
    }

    public void Stop()
    {
        if (!IsStarted) return;
        StopCoroutine(AutoTaskCoroutine);
        SetState(AutoTaskState.Idle);
        AutoTaskCoroutine = null;
    }

    private IEnumerator CoRun(IProgress<float> progress)
    {
        var remainingTasks = PlayerControl.LocalPlayer.GetRemainingTasks();
        while (remainingTasks.Count > 0 && LocalConditions.CanCompleteAutoTasks())
        {
            if (MeetingHud.Instance != null)
            {
                SetState(AutoTaskState.Paused);
                yield return new WaitForEndOfFrame();
                continue;
            }
            SetState(AutoTaskState.Running);
            var taskToComplete = remainingTasks.PickOneRandom();
            yield return CoCompleteTask(progress, taskToComplete);
            remainingTasks = PlayerControl.LocalPlayer != null ? PlayerControl.LocalPlayer.GetRemainingTasks() : [];
        }
        SetState(AutoTaskState.Completed);
        progress.Report(1f);
        AutoTaskCoroutine = null;
    }

    private IEnumerator CoCompleteTask(IProgress<float> progress, NormalPlayerTask task)
    {
        progress.Report(0f);
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
            progress.Report(timer / taskDuration);
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
            progress.Report(timer / taskDuration);
            yield return new WaitForEndOfFrame();
        }

        if (MeetingHud.Instance != null)
        {
            yield break;
        }
        
        progress.Report(1f);
        PlayerControl.LocalPlayer.BetterCompleteTask(task);
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
        return consolePositions._items[task.taskStep];
    }
}