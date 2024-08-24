using BetterVanilla.Components;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(GameStartManager))]
internal static class GameStartManagerPatches
{
    private static int PlayersCount { get; set; }
    
    [HarmonyPostfix, HarmonyPatch(nameof(GameStartManager.Start))]
    private static void StartPostfix()
    {
        BetterVanillaManager.Instance.Cheaters.SickoUsers.Clear();
        BetterVanillaManager.Instance.Cheaters.AumUsers.Clear();
    }
    
    [HarmonyPostfix, HarmonyPatch(nameof(GameStartManager.ReallyBegin))]
    private static void ReallyBeginPostfix()
    {
        PlayersCount = PlayerControl.AllPlayerControls.Count;
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(GameStartManager.Update))]
    private static bool UpdatePrefix(GameStartManager __instance)
    {
        if (!GameData.Instance || !GameManager.Instance)
        {
            return false;
        }
        if (!AmongUsClient.Instance || !AmongUsClient.Instance.AmHost) return true;
        if (__instance.LastPlayerCount != PlayersCount)
        {
            PlayersCount = __instance.LastPlayerCount;
        }
        var disableRequirementOption = BetterVanillaManager.Instance.LocalOptions.DisableGameStartRequirement;
        var playerRequirementDisabled = disableRequirementOption.IsLocked() || !disableRequirementOption.Value;

        var oldMinPlayers = __instance.MinPlayers;
        __instance.MinPlayers = playerRequirementDisabled ? 4 : 1;
        if (oldMinPlayers != __instance.MinPlayers)
        {
            __instance.LastPlayerCount--;
        }

        return true;
    }
}