using EnoPM.BetterVanilla.Core;
using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(GameStartManager))]
internal static class GameStartManagerPatches
{
    private static int PlayersCount { get; set; }

    [HarmonyPostfix, HarmonyPatch(nameof(GameStartManager.Start))]
    private static void StartPostfix()
    {
        CheatsManager.AumUsers.Clear();
        CheatsManager.SickoUsers.Clear();
        ModSettings.Host.UpdateAllSettings();
    }

    [HarmonyPostfix, HarmonyPatch(nameof(GameStartManager.ReallyBegin))]
    private static void ReallyBeginPostfix()
    {
        PlayersCount = PlayerControl.AllPlayerControls.Count;
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(GameStartManager.Update))]
    private static bool UpdatePrefix(GameStartManager __instance)
    {
        if (!GameData.Instance || !GameManager.Instance || !AmongUsClient.Instance || !AmongUsClient.Instance.AmHost) return false;
        if (__instance.LastPlayerCount != PlayersCount)
        {
            PlayersCount = __instance.LastPlayerCount;
            ModSettings.Host.UpdateAllSettings();
        }
        var playerRequirementDisabled = ModSettings.Local.DisableStartGamePlayerRequirement.IsLocked() || !ModSettings.Local.DisableStartGamePlayerRequirement;

        var oldMinPlayers = __instance.MinPlayers;
        __instance.MinPlayers = playerRequirementDisabled ? 4 : 1;
        if (oldMinPlayers != __instance.MinPlayers)
        {
            __instance.LastPlayerCount--;
        }

        return true;
    }
}