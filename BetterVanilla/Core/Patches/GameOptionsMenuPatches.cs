using BetterVanilla.Core.Extensions;
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
}