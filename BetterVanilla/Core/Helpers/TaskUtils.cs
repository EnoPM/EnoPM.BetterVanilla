using System;
using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options;

namespace BetterVanilla.Core.Helpers;

public static class TaskUtils
{
    public static bool ShouldRandomizeWireTaskLocation => HostOptions.Default.RandomizeFixWiringTaskOrder.IsAllowed() && HostOptions.Default.RandomizeFixWiringTaskOrder.Value;
    public static bool ShouldRandomizeUploadTaskLocation => HostOptions.Default.RandomizeUploadTaskLocation.IsAllowed() && HostOptions.Default.RandomizeUploadTaskLocation.Value;
    
    public static List<Console> PickRandomConsoles(TaskTypes taskType, byte[] consoleIds)
    {
        var consoles = GetAllConsoles(taskType);

        return PickRandomConsolesFrom(consoles, taskType, consoleIds);
    }

    public static List<Console> GetAllConsoles(TaskTypes taskType)
    {
        return ShipStatus.Instance.AllConsoles
            .Where(x => IsConsoleOfType(x ,taskType))
            .ToList();
    }

    public static bool IsConsoleOfType(Console console, TaskTypes taskType)
    {
        return console.TaskTypes.Contains(taskType) || console.ValidTasks.Any(x => x.taskType == taskType);
    }

    public static List<Console> PickRandomConsolesFrom(List<Console> consoles, TaskTypes taskType, byte[] consoleIds)
    {
        var cache = new List<Console>(consoles);

        for (var i = 0; i < consoleIds.Length; ++i)
        {
            if (i >= consoles.Count)
            {
                throw new IndexOutOfRangeException($"Could not find {consoleIds.Length} valid consoles for task {taskType.ToString()}");
            }
            var console = cache.PickOneRandom();
            consoleIds[i] = (byte)console.ConsoleId;
        }

        //Array.Sort(consoleIds);
        return consoles;
    }
}