using EnoPM.BetterVanilla.Extensions;
using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(MeetingHud))]
internal static class MeetingHudPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(MeetingHud.Update))]
    private static void UpdatePostfix(MeetingHud __instance)
    {
        foreach (var playerVote in __instance.playerStates)
        {
            playerVote.ModdedFixedUpdate();
        }
    }
}