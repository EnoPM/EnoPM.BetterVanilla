using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AmongUs.GameOptions;
using BepInEx;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options;

namespace BetterVanilla.Core;

public sealed class CustomTasksAssignation
{
    private int CommonTasksCount { get; set; }
    private int LongTasksCount { get; set; }
    private int ShortTasksCount { get; set; }

    private List<NormalPlayerTask> CommonTasks { get; }
    private List<NormalPlayerTask> LongTasks { get; }
    private List<NormalPlayerTask> ShortTasks { get; }

    private ShipStatus CurrentShipStatus { get; }

    public CustomTasksAssignation(ShipStatus shipStatus)
    {
        CurrentShipStatus = shipStatus;
        var options = GameOptionsManager.Instance.CurrentGameOptions;

        CommonTasksCount = options.TryGetInt(Int32OptionNames.NumCommonTasks, out var commonTasksCount) ? commonTasksCount : 0;
        LongTasksCount = options.TryGetInt(Int32OptionNames.NumLongTasks, out var longTasksCount) ? longTasksCount : 0;
        ShortTasksCount = options.TryGetInt(Int32OptionNames.NumShortTasks, out var shortTasksCount) ? shortTasksCount : 0;

        if (CommonTasksCount + LongTasksCount + ShortTasksCount == 0)
        {
            ShortTasksCount = 1;
        }

        CommonTasks = GetCommonTasks();
        LongTasks = GetLongTasks();
        ShortTasks = GetShortTasks();
    }

    public void Begin()
    {
        DumpTasksToFiles();
        CurrentShipStatus.numScans = 0;

        AssignTaskIndexes();

        PerformTaskMoving();

        CommonTasks.Shuffle();
        LongTasks.Shuffle();
        ShortTasks.Shuffle();

        var allPlayers = GameData.Instance.AllPlayers;
        var usedTaskTypes = new Il2CppSystem.Collections.Generic.HashSet<TaskTypes>();
        var tasks = new Il2CppSystem.Collections.Generic.List<byte>();

        var commonTasks = CommonTasks.ToIl2Cpp();
        var commonStart = 1;

        CurrentShipStatus.AddTasksFromList(ref commonStart, CommonTasksCount, tasks, usedTaskTypes, commonTasks);
        for (var i = 0; i < CommonTasksCount; ++i)
        {
            if (commonTasks.Count == 0)
            {
                Ls.LogWarning($"No enough common tasks");
                break;
            }
            var index = commonTasks.RandomIdx();
            tasks.Add((byte)index);
            commonTasks.RemoveAt(index);
        }

        var longTasks = LongTasks.ToIl2Cpp();
        var shortTasks = ShortTasks.ToIl2Cpp();

        var longStart = 0;
        var shortStart = 0;

        for (byte playerId = 0; playerId < allPlayers.Count; ++playerId)
        {
            usedTaskTypes.Clear();
            tasks.RemoveRange(CommonTasksCount, tasks.Count - CommonTasksCount);
            CurrentShipStatus.AddTasksFromList(ref longStart, LongTasksCount, tasks, usedTaskTypes, longTasks);
            CurrentShipStatus.AddTasksFromList(ref shortStart, ShortTasksCount, tasks, usedTaskTypes, shortTasks);

            var player = allPlayers[playerId];

            if (player != null && player && player.Object && !player.Object.GetComponent<DummyBehaviour>().enabled)
            {
                var array = tasks.ToArray().ToArray();
                player.RpcSetTasks(array);
            }
        }

        PlayerControl.LocalPlayer.cosmetics.SetAsLocalPlayer();
    }

    private void AssignTaskIndexes()
    {
        var index = 0;
        AssignIndexes(ref index, CommonTasks);
        AssignIndexes(ref index, LongTasks);
        AssignIndexes(ref index, ShortTasks);
    }

    private void DumpTasksToFiles()
    {
        if (!FeatureOptions.Default.DisableEndGameChecks.IsAllowed() || !FeatureOptions.Default.DisableEndGameChecks.Value)
        {
            return;
        }
        DumpTasksToFile(CurrentShipStatus.CommonTasks, "Common");
        DumpTasksToFile(CurrentShipStatus.LongTasks, "Long");
        DumpTasksToFile(CurrentShipStatus.ShortTasks, "Short");
    }

    private void DumpTasksToFile(IEnumerable<NormalPlayerTask> tasks, string fileName)
    {
        var filePath = Path.Combine(Paths.PluginPath, $"_{fileName}.txt");
        var sb = new StringBuilder();

        foreach (var task in tasks)
        {
            sb.Append($"TaskTypes.{task.TaskType.ToString()}, ");
        }

        File.WriteAllText(filePath, sb.ToString());
    }

    private void PerformTaskMoving()
    {
        TaskMover.Apply(CommonTasks, LongTasks, ShortTasks);

        if (CommonTasksCount > CommonTasks.Count)
        {
            Ls.LogWarning($"CommonTasksCount ({CommonTasksCount}) > available CommonTasks ({CommonTasks.Count}). Adjusting.");
            CommonTasksCount = CommonTasks.Count;
        }

        if (LongTasksCount > LongTasks.Count)
        {
            Ls.LogWarning($"LongTasksCount ({LongTasksCount}) > available LongTasks ({LongTasks.Count}). Adjusting.");
            LongTasksCount = LongTasks.Count;
        }

        if (ShortTasksCount > ShortTasks.Count)
        {
            Ls.LogWarning($"ShortTasksCount ({ShortTasksCount}) > available ShortTasks ({ShortTasks.Count}). Adjusting.");
            ShortTasksCount = ShortTasks.Count;
        }
    }

    private NormalPlayerTask[] PickTasks(TaskTypes taskType)
    {
        var results = new List<NormalPlayerTask>();
        var commonTasks = CommonTasks.Where(t => t.TaskType == taskType).ToArray();
        foreach (var task in commonTasks)
        {
            CommonTasks.Remove(task);
        }
        results.AddRange(commonTasks);
        
        var longTasks = LongTasks.Where(t => t.TaskType == taskType).ToArray();
        foreach (var task in longTasks)
        {
            LongTasks.Remove(task);
        }
        results.AddRange(longTasks);
        
        var shortTasks = ShortTasks.Where(t => t.TaskType == taskType).ToArray();
        foreach (var task in shortTasks)
        {
            ShortTasks.Remove(task);
        }
        results.AddRange(shortTasks);
        
        return results.ToArray();
    }

    private List<NormalPlayerTask> GetCommonTasks()
    {
        var tasks = CurrentShipStatus.CommonTasks.ToList();
        SetTasksLength(tasks, NormalPlayerTask.TaskLength.Short);
        return tasks;
    }

    private List<NormalPlayerTask> GetLongTasks()
    {
        var tasks = CurrentShipStatus.LongTasks.ToList();
        SetTasksLength(tasks, NormalPlayerTask.TaskLength.Short);
        return tasks;
    }

    private List<NormalPlayerTask> GetShortTasks()
    {
        var tasks = CurrentShipStatus.ShortTasks.ToList();
        SetTasksLength(tasks, NormalPlayerTask.TaskLength.Short);
        return tasks;
    }

    private static void SetTasksLength(List<NormalPlayerTask> tasks, NormalPlayerTask.TaskLength taskLength)
    {
        foreach (var task in tasks)
        {
            task.Length = taskLength;
        }
    }

    private static void AssignIndexes(ref int index, List<NormalPlayerTask> tasks)
    {
        foreach (var task in tasks)
        {
            task.Index = index++;
        }
    }
}