using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using EnoPM.BetterVanilla.Core.Extensions;
using UnityEngine;

namespace EnoPM.BetterVanilla.Components;

public class AutoTaskFinisher : MonoBehaviour
{
    private readonly List<uint> _tasksDone = [];
    
    private void Start()
    {
        this.StartCoroutine(CoFinishMyTasks());
    }

    private IEnumerator CoFinishMyTasks()
    {
        var remainingTasks = GetRemainingTasks();
        while (remainingTasks.Count > 0)
        {
            while (ModSettings.Local.AutoFinishMyTasks.IsLocked() || !ModSettings.Local.AutoFinishMyTasks)
            {
                yield return Pause();
            }
            var task = remainingTasks.PickOneRandom();
            var normalTask = task.TryCast<NormalPlayerTask>();
            if (normalTask != null)
            {
                normalTask.taskStep = normalTask.MaxStep;
            }
            PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
            _tasksDone.Add(task.Id);
            yield return Pause();
            remainingTasks = GetRemainingTasks();
        }
        Plugin.Logger.LogMessage($"All tasks finished!");
    }

    private List<PlayerTask> GetRemainingTasks()
    {
        var player = PlayerControl.LocalPlayer;
        if (player && player.myTasks != null)
        {
            return player.myTasks.ToArray()
                .Where(x => !x.IsComplete && !_tasksDone.Contains(x.Id))
                .ToList();
        }

        return [];
    }
    
    private static IEnumerator Pause()
    {
        var time = Random.RandomRange(1f, 10f);
        yield return new WaitForSeconds(time);
        if (MeetingHud.Instance)
        {
            yield return Pause();
        }
    }
}