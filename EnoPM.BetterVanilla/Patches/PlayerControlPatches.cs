using System.Collections.Generic;
using AmongUs.Data;
using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Core.Extensions;
using HarmonyLib;
using Hazel;
using UnityEngine;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(PlayerControl))]
internal static class PlayerControlPatches
{
    internal static readonly List<int> CheaterOwnerIds = [];
    
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerControl.FixedUpdate))]
    private static void FixedUpdatePostfix(PlayerControl __instance)
    {
        if (!__instance) return;
        __instance.ModdedFixedUpdate();
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PlayerControl.HandleRpc))]
    private static void HandleRpcPostfix(PlayerControl __instance, byte callId, MessageReader reader)
    {
        switch (callId)
        {
            case ChatControllerPatches.PrivateMessageRpcId:
                ChatControllerPatches.HandlePrivateMessageRpc(__instance, reader);
                break;
            case 85:
                if (!CheaterOwnerIds.Contains(__instance.OwnerId))
                {
                    CheaterOwnerIds.Add(__instance.OwnerId);
                }
                break;
        }
    }

    [HarmonyPrefix, HarmonyPatch(nameof(PlayerControl.StartMeeting))]
    private static void StartMeetingPrefix()
    {
        if (ZoomBehaviour.Instance)
        {
            ZoomBehaviour.Instance.ResetZoom();
        }
    }

    [HarmonyPrefix, HarmonyPatch(nameof(PlayerControl.RpcSetLevel))]
    private static void RpcSetLevelPrefix(PlayerControl __instance, ref uint level)
    {
        if (!__instance.AmOwner) return;
        if (DB.Player.PlayerExp < DataManager.Player.Stats.Xp)
        {
            DB.Player.PlayerExp += DataManager.Player.Stats.Xp;
            DB.Player.PlayerLevel = XpManager.CalculateLevel(DB.Player.PlayerExp);
            DB.SavePlayer();
        }
        level = DB.Player.PlayerLevel;
    }
}