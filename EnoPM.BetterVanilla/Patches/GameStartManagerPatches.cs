using AmongUs.GameOptions;
using HarmonyLib;
using InnerNet;
using UnityEngine;

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

    [HarmonyPostfix, HarmonyPatch(nameof(GameStartManager.ReallyBegin))]
    private static void ReallyBeginPostfix()
    {
        PlayersCount = PlayerControl.AllPlayerControls.Count;
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(GameStartManager.Update))]
    private static bool UpdatePrefix(GameStartManager __instance)
    {
        if (!GameData.Instance || !GameManager.Instance) return false;
        
        __instance.MinPlayers = 1;

        return true;
    }
}