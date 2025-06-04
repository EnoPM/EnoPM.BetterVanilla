using BetterVanilla.Cosmetics.Extensions;
using HarmonyLib;

namespace BetterVanilla.Cosmetics.Patches;

[HarmonyPatch(typeof(HatsTab))]
internal static class HatsTabPatches
{
    
    [HarmonyPrefix, HarmonyPatch(nameof(HatsTab.OnEnable))]
    private static bool OnEnablePrefix(HatsTab __instance)
    {
        __instance.SetupCustomHats();
        return false;
    }
}