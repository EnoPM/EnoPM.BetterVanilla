using BetterVanilla.Core;
using HarmonyLib;

namespace BetterVanilla.Cosmetics.Core.Patches;

[HarmonyPatch(typeof(PlayerControl))]
internal static class PlayerControlPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(PlayerControl.RawSetVisor))]
    private static bool SetVisorPrefix(PlayerControl __instance, string visorId, int color)
    {
        Ls.LogMessage($"Set Visor {visorId} for {__instance.Data?.PlayerName}");
        return true;
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(PlayerControl.RawSetHat))]
    private static bool SetHatPrefix(PlayerControl __instance, string hatId, int colorId)
    {
        Ls.LogMessage($"Set Hat {hatId} for {__instance.Data?.PlayerName}");
        return true;
    }
}