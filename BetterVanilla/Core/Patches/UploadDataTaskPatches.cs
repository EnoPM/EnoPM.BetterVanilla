using BetterVanilla.Core.Helpers;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(UploadDataTask))]
internal static class UploadDataTaskPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(UploadDataTask.ValidConsole))]
    private static bool ValidConsolePrefix(UploadDataTask __instance, Console console, ref bool __result)
    {
        if (!TaskUtils.ShouldRandomizeUploadTaskLocation) return true;
        var consoleId = __instance.Data[__instance.TaskStep];
        var room = __instance.TaskStep == 1 ? __instance.EndAt : __instance.StartAt;
        
        __result = console.ConsoleId == consoleId && console.Room == room && TaskUtils.IsConsoleOfType(console, __instance.TaskType);

        return false;
    }
}