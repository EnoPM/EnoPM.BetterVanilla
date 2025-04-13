using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class TaskFinisherBehaviour : MonoBehaviour
{
    private Coroutine Routine { get; set; }
    private static Dictionary<TaskTypes, AutoTaskData> Durations => BetterVanillaManager.Instance.Features.Registry?.TaskDurations ?? new Dictionary<TaskTypes, AutoTaskData>();

    public bool CanBeStarted()
    {
        return PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.CanFinishTask() && Routine == null;
    }

    private void Awake()
    {
        if (BetterVanillaManager.Instance.TaskFinisher)
        {
            throw new Exception("TaskFinisherBehaviour must be a singleton.");
        }

        BetterVanillaManager.Instance.TaskFinisher = this;
    }

    private static void SetUiTaskName(string taskName)
    {
        if (!BetterVanillaManager.Instance || !BetterVanillaManager.Instance.Menu || !BetterVanillaManager.Instance.Menu.HomeTab) return;
        BetterVanillaManager.Instance.Menu.HomeTab.SetCurrentTaskName(taskName);
    }

    private static void SetUiProgression(float progression)
    {
        if (!BetterVanillaManager.Instance || !BetterVanillaManager.Instance.Menu || !BetterVanillaManager.Instance.Menu.HomeTab) return;
        BetterVanillaManager.Instance.Menu.HomeTab.SetTaskProgression(progression);
    }

    private void Start()
    {
        SetUiTaskName("Not available");
        SetUiProgression(0f);
    }

    public void Run()
    {
        Ls.LogInfo($"Running TaskFinisherBehaviour...");
        if (Routine != null) return;
        Routine = this.StartCoroutine(CoRun());
    }

    private bool IsTaskDoable()
    {
        return !MeetingHud.Instance;
    }

    private IEnumerator CoRun()
    {
        BetterVanillaManager.Instance.Menu.HomeTab.SetTaskProgression(0f);
        var remainingTasks = PlayerControl.LocalPlayer.GetRemainingTasks();
        while (remainingTasks.Count > 0 && PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.CanFinishTask())
        {
            if (!IsTaskDoable())
            {
                SetUiTaskName("Not available during meeting");
                yield return new WaitForEndOfFrame();
                continue;
            }
            var task = remainingTasks.PickOneRandom();
            yield return CoDoTask(task);
            remainingTasks = PlayerControl.LocalPlayer ? PlayerControl.LocalPlayer.GetRemainingTasks() : [];
        }
        SetUiProgression(1f);
        SetUiTaskName("All tasks completed");
    }

    private static float GetTravelTimeToTask(NormalPlayerTask task)
    {
        var consolePositions = task.FindConsolesPos();
        var playerPosition = PlayerControl.LocalPlayer.transform.position;
        var currentConsolePosition = consolePositions._items[task.taskStep];
        var speed = PlayerControl.LocalPlayer.rigidbody2D.velocity.magnitude;

        if (speed <= 0f)
        {
            return 10f;
        }
        
        return Vector2.Distance(playerPosition, currentConsolePosition) / speed;
    }

    private IEnumerator CoDoTask(NormalPlayerTask task)
    {
        var duration = Durations.TryGetValue(task.TaskType, out var taskData) ? taskData : new AutoTaskData
        {
            Duration = 15f,
        };
        SetUiTaskName(TranslationController.Instance.GetString(task.TaskType));
        var maxTimer = duration.GetDuration() + (task.MaxStep - 1) * (duration.StepDelay ?? 0f) + GetTravelTimeToTask(task);
        var timer = maxTimer;
        Ls.LogMessage($"{nameof(TaskFinisherBehaviour)}: Task {task.TaskType.ToString()} will be completed in {timer}s.");
        SetUiProgression(0f);
        while (timer > 0f && IsTaskDoable())
        {
            if (!IsTaskDoable())
            {
                break;
            }
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
            SetUiProgression(1f - timer / maxTimer);
        }

        if (timer > 0f || !IsTaskDoable())
        {
            SetUiProgression(0f);
            yield break;
        }

        SetUiProgression(1f);
        yield return new WaitForSeconds(1f);
        
        PlayerControl.LocalPlayer.BetterCompleteTask(task);
    }
}