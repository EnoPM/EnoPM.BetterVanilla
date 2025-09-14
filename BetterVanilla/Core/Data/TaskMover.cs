using System;
using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Options;

namespace BetterVanilla.Core.Data;

public static class TaskMover
{
    public static HashSet<TaskTypes> CommonTasks { get; } = [];
    public static HashSet<TaskTypes> LongTasks { get; } = [];
    public static HashSet<TaskTypes> ShortTasks { get; } = [];

    private static void MoveToCommonTasks(TaskTypes taskType)
    {
        LongTasks.Remove(taskType);
        ShortTasks.Remove(taskType);
        CommonTasks.Add(taskType);
    }

    private static void MoveToLongTasks(TaskTypes taskType)
    {
        CommonTasks.Remove(taskType);
        ShortTasks.Remove(taskType);
        LongTasks.Add(taskType);
    }

    private static void MoveToShortTasks(TaskTypes taskType)
    {
        CommonTasks.Remove(taskType);
        LongTasks.Remove(taskType);
        ShortTasks.Add(taskType);
    }

    public static void Refresh()
    {
        if (HostOptions.Default.DefineCommonTasksAsNonCommon.Value)
        {
            MoveToLongTasks(TaskTypes.FixWiring);
            MoveToShortTasks(TaskTypes.SwipeCard);
            MoveToShortTasks(TaskTypes.InsertKeys);
            MoveToShortTasks(TaskTypes.ScanBoardingPass);
            MoveToShortTasks(TaskTypes.EnterIdCode);
            MoveToShortTasks(TaskTypes.CollectSamples);
            MoveToShortTasks(TaskTypes.ReplaceParts);
            MoveToShortTasks(TaskTypes.RoastMarshmallow);
        }
        else
        {
            MoveToCommonTasks(TaskTypes.FixWiring);
            MoveToCommonTasks(TaskTypes.SwipeCard);
            MoveToCommonTasks(TaskTypes.InsertKeys);
            MoveToCommonTasks(TaskTypes.ScanBoardingPass);
            MoveToCommonTasks(TaskTypes.EnterIdCode);
            MoveToCommonTasks(TaskTypes.CollectSamples);
            MoveToCommonTasks(TaskTypes.ReplaceParts);
            MoveToCommonTasks(TaskTypes.RoastMarshmallow);
        }
    }

    public static void Apply(List<TaskTypes> commonTasks, List<TaskTypes> longTasks, List<TaskTypes> shortTasks)
    {
        foreach (var taskType in CommonTasks)
        {
            commonTasks.AddRange(PickTasksFrom(taskType, longTasks, shortTasks));
        }

        foreach (var taskType in LongTasks)
        {
            longTasks.AddRange(PickTasksFrom(taskType, commonTasks, shortTasks));
        }

        foreach (var taskType in ShortTasks)
        {
            shortTasks.AddRange(PickTasksFrom(taskType, commonTasks, longTasks));
        }
    }

    public static void Apply(List<NormalPlayerTask> commonTasks, List<NormalPlayerTask> longTasks, List<NormalPlayerTask> shortTasks)
    {
        foreach (var taskType in CommonTasks)
        {
            commonTasks.AddRange(PickTasksFrom(taskType, longTasks, shortTasks));
        }

        foreach (var taskType in LongTasks)
        {
            longTasks.AddRange(PickTasksFrom(taskType, commonTasks, shortTasks));
        }

        foreach (var taskType in ShortTasks)
        {
            shortTasks.AddRange(PickTasksFrom(taskType, commonTasks, longTasks));
        }
    }

    private static TaskTypes[] PickTasksFrom(TaskTypes taskType, params List<TaskTypes>[] taskLists) =>
        PickTasksFrom(taskType, x => x, taskLists);

    private static NormalPlayerTask[] PickTasksFrom(TaskTypes taskType, params List<NormalPlayerTask>[] taskLists) =>
        PickTasksFrom(taskType, x => x.TaskType, taskLists);

    private static T[] PickTasksFrom<T>(TaskTypes taskType, Func<T, TaskTypes> taskTypeSelector, params List<T>[] taskLists)
    {
        var results = new List<T>();

        foreach (var taskList in taskLists)
        {
            var matches = taskList.Where(x => taskTypeSelector(x) == taskType).ToArray();
            foreach (var task in matches)
            {
                taskList.Remove(task);
            }
            results.AddRange(matches);
        }

        return results.ToArray();
    }
}