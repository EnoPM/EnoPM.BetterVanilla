using System.Linq;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Core.Helpers;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(NormalPlayerTask))]
internal static class NormalPlayerTaskPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(NormalPlayerTask.ValidConsole))]
    private static bool ValidConsolePrefix(NormalPlayerTask __instance, Console console, ref bool __result)
    {
        if (__instance.TaskType == TaskTypes.RecordTemperature)
        {
            return IsValidRecordTemperatureConsole(__instance, console, ref __result);
        }
        return true;
    }

    [HarmonyPostfix, HarmonyPatch(nameof(NormalPlayerTask.Initialize))]
    private static void InitializePostfix(NormalPlayerTask __instance)
    {
        if (__instance.TaskType == TaskTypes.FixWiring)
        {
            InitializeFixWiringTask(__instance);
        }
        else if (__instance.TaskType == TaskTypes.UploadData)
        {
            InitializeUploadDataTask(__instance);
        }
        else if (__instance.TaskType == TaskTypes.RecordTemperature)
        {
            InitializeRecordTemperatureTask(__instance);
        }
    }

    private static void InitializeRecordTemperatureTask(NormalPlayerTask task)
    {
        if (ShipStatus.Instance.Type != ShipStatus.MapType.Pb || !LocalConditions.IsBetterPolusEnabled())
        {
            return;
        }
        var consoles = TaskUtils.GetAllConsoles(task.TaskType);
        for (var i = 0; i < consoles.Count; i++)
        {
            consoles[i].ConsoleId = i;
        }

        var data = new byte[1];
        var randomConsoles = TaskUtils.PickRandomConsolesFrom(consoles, task.TaskType, data);
        
        task.Data = data;
        task.StartAt = randomConsoles.First(x => x.ConsoleId == data[0]).Room;
    }

    private static void InitializeUploadDataTask(NormalPlayerTask task)
    {
        if (!TaskUtils.ShouldRandomizeUploadTaskLocation) return;
        if (!task.Is<UploadDataTask>(out var uploadDataTask)) return;
        
        var consoles = TaskUtils.GetAllConsoles(uploadDataTask.TaskType);
        
        for (var i = 0; i < consoles.Count; i++)
        {
            consoles[i].ConsoleId = i;
        }
        
        var data = new byte[2];
        var randomConsoles = TaskUtils.PickRandomConsolesFrom(consoles, uploadDataTask.TaskType, data);
        
        uploadDataTask.Data = data;
        uploadDataTask.StartAt = randomConsoles.First(x => x.ConsoleId == data[0]).Room;
        uploadDataTask.EndAt = randomConsoles.First(x => x.ConsoleId == data[1]).Room;
        
        Ls.LogMessage($"Download {uploadDataTask.StartAt.ToString()}, Upload: {uploadDataTask.EndAt.ToString()}");
    }

    private static void InitializeFixWiringTask(NormalPlayerTask task)
    {
        if (!TaskUtils.ShouldRandomizeWireTaskLocation) return;
        var data = new byte[task.MaxStep];
        var randomConsoles = TaskUtils.PickRandomConsoles(TaskTypes.FixWiring, data);
        task.Data = data;
        task.StartAt = randomConsoles.First(x => x.ConsoleId == data[0]).Room;
    }

    private static bool IsValidRecordTemperatureConsole(NormalPlayerTask task, Console console, ref bool result)
    {
        if (ShipStatus.Instance.Type != ShipStatus.MapType.Pb || !LocalConditions.IsBetterPolusEnabled())
        {
            return true;
        }

        var consoleId = task.Data[0];
        
        result = console.ConsoleId == consoleId && console.Room == task.StartAt && TaskUtils.IsConsoleOfType(console, task.TaskType);
        return false;
    }
}