using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
using BetterVanilla.Core.Extensions;
using HarmonyLib;
using Hazel;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(PlayerControl))]
internal static class PlayerControlPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerControl.HandleRpc))]
    private static void HandleRpcPostfix(PlayerControl __instance, byte callId, MessageReader reader)
    {
        if (callId == PlayerControlRpcExtensions.ReservedRpcCallId)
        {
            __instance.HandleCustomRpc(reader);
        }
        else
        {
            BetterVanillaManager.Instance.Cheaters.HandleRpc(__instance, callId, reader);
        }
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
}