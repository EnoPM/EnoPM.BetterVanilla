using BetterVanilla.Components;
using BetterVanilla.Core;
using HarmonyLib;

namespace BetterVanilla.Cosmetics.Core.Patches;

[HarmonyPatch(typeof(PlayerControl))]
internal static class PlayerControlPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerControl.RawSetVisor))]
    private static void RawSetVisorPrefix(PlayerControl __instance, string visorId, int color)
    {
        Ls.LogMessage($"Set Visor {visorId} for {__instance.Data?.PlayerName}");
    }
    
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerControl.RawSetHat))]
    private static void RawSetHatPostfix(PlayerControl __instance, string hatId, int colorId)
    {
        Ls.LogMessage($"Set Hat {hatId} for {__instance.Data?.PlayerName}");
        __instance.gameObject.GetComponent<BetterPlayerControl>().RefreshHatColor();
    }
}