using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
using BetterVanilla.Core.Extensions;
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

        var oldMinPlayers = __instance.MinPlayers;
        __instance.MinPlayers = LocalConditions.ShouldDisableGameStartRequirement() ? 1 : 4;
        if (oldMinPlayers != __instance.MinPlayers)
        {
            __instance.LastPlayerCount--;
        }

        return true;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(GameStartManager.UpdateHostPanelImage))]
    private static bool UpdateHostPanelImagePrefix(GameStartManager __instance, NetworkedPlayerInfo player)
    {
        __instance.StartCoroutine(__instance.CoUpdateHostPanelImage(player));
        return false;
    }

    private static IEnumerator CoUpdateHostPanelImage(this GameStartManager gameStartManager, NetworkedPlayerInfo playerInfo)
    {
        yield return gameStartManager.HostInfoPanel.SetCosmetics(playerInfo);
        var player = BetterVanillaManager.Instance.GetPlayerById(playerInfo.PlayerId);
        if (player == null)
        {
            yield break;
        }
        gameStartManager.HostInfoPanel.player.SetVisorColor(player.GetVisorColor());
    }
}