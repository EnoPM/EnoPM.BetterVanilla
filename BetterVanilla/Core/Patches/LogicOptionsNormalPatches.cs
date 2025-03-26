using BetterVanilla.Components;
using BetterVanilla.Core.Helpers;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(LogicOptionsNormal))]
internal static class LogicOptionsNormalPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(LogicOptionsNormal.GetAnonymousVotes))]
    private static void GetAnonymousVotesPostfix(ref bool __result)
    {
        if (__result && LocalConditions.ShouldRevealVoteColors())
        {
            __result = false;
        }
    }
}