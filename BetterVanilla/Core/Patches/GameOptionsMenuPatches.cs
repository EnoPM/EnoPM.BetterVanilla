using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options;
using BetterVanilla.Options.Core;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(GameOptionsMenu))]
internal static class GameOptionsMenuPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(GameOptionsMenu.CreateSettings))]
    private static bool CreateSettingsPrefix(GameOptionsMenu __instance)
    {
        __instance.CreateBetterSettings();
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(GameOptionsMenu.ValueChanged))]
    private static bool ValueChangedPrefix(GameOptionsMenu __instance, OptionBehaviour option)
    {
        return __instance.CustomValueChanged(option);
    }

    [HarmonyPostfix, HarmonyPatch(nameof(GameOptionsMenu.Update))]
    private static void UpdatePostfix(GameOptionsMenu __instance)
    {
        foreach (var option in HostOptions.Default.GetOptions())
        {
            option.UpdateBehaviour();
        }
    }

    [HarmonyPostfix, HarmonyPatch(nameof(GameOptionsMenu.Initialize))]
    private static void InitializePostfix(GameOptionsMenu __instance)
    {
        var currentMapTasks = MapTasks.Current;
        currentMapTasks?.RefreshOptions();
    }
}