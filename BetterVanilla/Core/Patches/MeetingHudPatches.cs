using System.Linq;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options;
using HarmonyLib;
using UnityEngine;

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

    [HarmonyPrefix, HarmonyPatch(nameof(MeetingHud.SortButtons))]
    private static bool SortButtonsPrefix(MeetingHud __instance)
    {
        if (HostOptions.Default.RandomizePlayerOrderInMeetings.IsNotAllowed() || !HostOptions.Default.RandomizePlayerOrderInMeetings.Value)
        {
            return true;
        }
        
        var allVoteAreas = __instance.playerStates.ToList();
        allVoteAreas.Shuffle();
        var orderedVoteAreas = allVoteAreas
            .OrderBy(x => x.DidReport ? 1 : x.AmDead ? 3 : 2).ToList();

        for (var i = 0; i < orderedVoteAreas.Count; ++i)
        {
            var item = orderedVoteAreas[i];
            var num1 = i % 3;
            var num2 = i / 3;
            
            var positionOffset = new Vector3(__instance.VoteButtonOffsets.x * num1, __instance.VoteButtonOffsets.y * num2, (float)(-0.8999999761581421 - num2 * 0.009999999776482582));
            item.transform.localPosition = __instance.VoteOrigin + positionOffset;
        }
        
        return false;
    }
}