using BetterVanilla.Components;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(PlayerVoteArea))]
internal static class PlayerVoteAreaPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerVoteArea.Start))]
    private static void StartPostfix(PlayerVoteArea __instance)
    {
        __instance.gameObject.AddComponent<BetterPlayerVoteArea>();
    }
}