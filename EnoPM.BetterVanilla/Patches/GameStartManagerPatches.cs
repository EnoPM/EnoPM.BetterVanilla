using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(GameStartManager))]
internal static class GameStartManagerPatches
{
    public static int PlayersCount { get; private set; }

    [HarmonyPostfix, HarmonyPatch(nameof(GameStartManager.Start))]
    private static void StartPostfix()
    {
        PlayerControlPatches.CheaterOwnerIds.Clear();
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(GameStartManager.Update))]
    private static void UpdatePrefix(GameStartManager __instance)
    {
        __instance.MinPlayers = 1;
    }

    [HarmonyPostfix, HarmonyPatch(nameof(GameStartManager.ReallyBegin))]
    private static void ReallyBeginPostfix()
    {
        PlayersCount = PlayerControl.AllPlayerControls.Count;
    }
}