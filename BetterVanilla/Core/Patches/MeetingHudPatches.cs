using BetterVanilla.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(MeetingHud))]
internal static class MeetingHudPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(MeetingHud.CastVote))]
    private static void CastVotePostfix(MeetingHud __instance, byte srcPlayerId, byte suspectPlayerId)
    {
        __instance.BetterCastVote(srcPlayerId, suspectPlayerId);
    }

    [HarmonyPostfix, HarmonyPatch(nameof(MeetingHud.Update))]
    private static void UpdatePostfix(MeetingHud __instance)
    {
        __instance.BetterUpdate();
    }

    [HarmonyPostfix, HarmonyPatch(nameof(MeetingHud.Start))]
    private static void StartPostfix(MeetingHud __instance)
    {
        __instance.BetterStart();
        __instance.HideDeadPlayerPets();
    }
}