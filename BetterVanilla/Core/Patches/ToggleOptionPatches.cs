using AmongUs.GameOptions;
using BetterVanilla.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(ToggleOption))]
internal static class ToggleOptionPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(ToggleOption.Initialize))]
    private static bool InitializePrefix(ToggleOption __instance)
    {
        return __instance.Title != StringNames.None && __instance.boolOptionName != BoolOptionNames.Invalid;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(ToggleOption.UpdateValue))]
    private static bool UpdateValuePrefix(ToggleOption __instance)
    {
        return __instance.CustomUpdateValue();
    }
}