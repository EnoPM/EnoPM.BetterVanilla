using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
using BetterVanilla.Core.Extensions;
using HarmonyLib;
using Hazel;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(PlayerControl))]
internal static class PlayerControlPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(PlayerControl.HandleRpc))]
    private static bool HandleRpcPostfix(PlayerControl __instance, byte callId, MessageReader reader)
    {
        if (callId == CustomRpcMessage.ReservedRpcCallId)
        {
            CustomRpcMessage.HandleRpcMessage(__instance, reader);
            return false;
        }
        BetterVanillaManager.Instance.Cheaters.HandleRpc(__instance, callId, reader);
        return true;
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PlayerControl.Awake))]
    private static void AwakePostfix(PlayerControl __instance)
    {
        if (__instance.notRealPlayer) return;
        GameEventManager.TriggerPlayerJoined(__instance);
    }

    [HarmonyPrefix, HarmonyPatch(nameof(PlayerControl.Start))]
    private static bool StartPrefix(PlayerControl __instance)
    {
        __instance.StartCoroutine(__instance.CoBetterStart());
        return false;
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PlayerControl.StartMeeting))]
    private static void StartMeetingPostfix(PlayerControl __instance)
    {
        GameEventManager.TriggerMeetingStarted();
    }

    [HarmonyPrefix, HarmonyPatch(nameof(PlayerControl.CheckMurder))]
    private static bool CheckMurderPrefix(PlayerControl __instance, PlayerControl target)
    {
        // Avec DisableServerAuthority, l'hôte gère les checks côté client
        Ls.LogMessage($"CheckMurderPrefix called - {__instance.Data?.PlayerName} attempting to kill {target?.Data?.PlayerName}");
        
        __instance.isKilling = false;
        if (AmongUsClient.Instance.IsGameOver || !AmongUsClient.Instance.AmHost)
        {
            return false;
        }
        
        if (target == null || __instance.Data.IsDead || !__instance.Data.Role.IsImpostor || __instance.Data.Disconnected)
        {
            __instance.RpcMurderPlayer(target, false);
            return false;
        }
        
        var data = target.Data;
        if (data == null || data.IsDead || target.inVent || target.MyPhysics.Animations.IsPlayingEnterVentAnimation() || target.MyPhysics.Animations.IsPlayingAnyLadderAnimation() || target.inMovingPlat)
        {
            __instance.RpcMurderPlayer(target, false);
            return false;
        }
        
        if (MeetingHud.Instance != null)
        {
            __instance.RpcMurderPlayer(target, false);
            return false;
        }
        
        // Vérification de protection BetterVanilla
        var betterTarget = target.gameObject.GetComponent<BetterPlayerControl>();
        if (betterTarget != null && betterTarget.IsProtected && PlayerShieldBehaviour.Instance.IsPlayerProtected(target))
        {
            Ls.LogMessage($"Blocked murder attempt on protected player: {target.Data?.PlayerName}");
            __instance.RpcMurderPlayer(target, false);
            return false;
        }
        
        // Meurtre autorisé
        __instance.isKilling = true;
        __instance.RpcMurderPlayer(target, true);
        return false;
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PlayerControl.MurderPlayer))]
    private static void MurderPlayerPostfix(PlayerControl __instance, PlayerControl target, MurderResultFlags resultFlags)
    {
        if (target == null || target.Data == null || __instance == null || __instance.Data == null) return;
        if (LocalConditions.AmDead())
        {
            Ls.LogMessage($"{__instance.Data.PlayerName} killed {target.Data.PlayerName}: {resultFlags.ToString()}");
        }
        if (resultFlags != MurderResultFlags.Succeeded && resultFlags != MurderResultFlags.DecisionByHost) return;
        PlayerShieldBehaviour.Instance.SetKilledPlayer(target);
    }
}