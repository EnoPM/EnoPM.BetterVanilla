using BetterVanilla.Components;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(PlayerVoteArea))]
internal static class PlayerVoteAreaPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerVoteArea.Start))]
    private static void StartPostfix(PlayerVoteArea __instance)
    {
        if (__instance.PlayerIcon == null || MeetingHud.Instance == null)
        {
            return;
        }
        Ls.LogMessage($"Adding BetterPlayerVoteArea for player id : {__instance.TargetPlayerId}");
        __instance.gameObject.AddComponent<BetterPlayerVoteArea>();
    }
}