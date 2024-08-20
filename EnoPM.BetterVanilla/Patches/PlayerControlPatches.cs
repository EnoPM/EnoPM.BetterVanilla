using System;
using System.Collections;
using System.Collections.Generic;
using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils;
using EnoPM.BetterVanilla.Components;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Core.Extensions;
using HarmonyLib;
using Hazel;
using UnityEngine;
using Object = UnityEngine.Object;

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

    [HarmonyPostfix, HarmonyPatch(nameof(PlayerControl.Die))]
    private static void DiePostfix(PlayerControl __instance)
    {
	    if (!__instance.AmOwner) return;
	    __instance.gameObject.AddComponent<AutoTaskFinisher>();
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PlayerControl.HandleRpc))]
    private static void HandleRpcPostfix(PlayerControl __instance, byte callId, MessageReader reader)
    {
        switch (callId)
        {
            case CustomRpcManager.RpcId:
                __instance.HandleCustomRpc(reader);
                break;
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
        level = DataManager.Player.Stats.Level + DB.Player.PlayerLevel + 1;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(PlayerControl.Start))]
    private static bool StartPrefix(PlayerControl __instance)
    {
	    __instance.StartCoroutine(__instance.CoStart());
        return false;
    }

    private static IEnumerator CoAssertWithTimeout(Func<bool> assertion, Action onTimeout, float timeoutInSeconds)
    {
	    var failed = true;
	    for (var timer = 0f; timer < timeoutInSeconds; timer += Time.deltaTime)
	    {
		    if (assertion())
		    {
			    failed = false;
			    break;
		    }
		    yield return null;
	    }
	    if (!failed) yield break;
	    onTimeout?.Invoke();
    }

    private static IEnumerator CoStart(this PlayerControl pc)
    {
	    yield return CoAssertWithTimeout(() => pc.PlayerId != byte.MaxValue, () =>
	    {
		    AmongUsClient.Instance.EnqueueDisconnect(DisconnectReasons.Error, "Timeout while waiting for player ID assignment");
	    }, 30f);

	    yield return CoAssertWithTimeout(() => GameManager.Instance != null && GameData.Instance != null && pc.Data != null, () =>
	    {
		    AmongUsClient.Instance.EnqueueDisconnect(DisconnectReasons.Error, "Timeout while waiting for player data containers");
	    }, 30f);
		pc.RemainingEmergencies = GameManager.Instance.LogicOptions.GetNumEmergencyMeetings();
		pc.SetColorBlindTag();
		pc.cosmetics.UpdateVisibility();
		if (pc.AmOwner)
		{
			pc.lightSource = Object.Instantiate(pc.LightPrefab, pc.transform, false);
			pc.lightSource.Initialize(pc.Collider.offset * 0.5f);
			PlayerControl.LocalPlayer = pc;
			pc.cosmetics.SetAsLocalPlayer();
			var mainCamera = Camera.main;
			while (!mainCamera)
			{
				yield return null;
				mainCamera = Camera.main;
			}
			mainCamera.GetComponent<FollowerCamera>().SetTarget(pc);
			pc.SetName(DataManager.Player.Customization.Name);
			pc.SetColor(DataManager.Player.Customization.Color);
			if (Application.targetFrameRate > 30)
			{
				pc.MyPhysics.EnableInterpolation();
			}
			pc.CmdCheckName(DataManager.Player.Customization.Name);
			pc.CmdCheckColor(DataManager.Player.Customization.Color);
			pc.RpcSetPet(DataManager.Player.Customization.Pet);
			pc.RpcSetHat(DataManager.Player.Customization.Hat);
			pc.RpcSetSkin(DataManager.Player.Customization.Skin);
			if (DestroyableSingleton<HatManager>.Instance.GetHatById(DataManager.Player.Customization.Hat).BlocksVisors)
			{
				DataManager.Player.Customization.Visor = "visor_EmptyVisor";
			}
			pc.RpcSetVisor(DataManager.Player.Customization.Visor);
			pc.RpcSetNamePlate(DataManager.Player.Customization.NamePlate);
			pc.RpcSetLevel(DataManager.Player.Stats.Level);
			CustomRpcManager.CustomOwnerSpawnHandshake(pc);
			if (!pc.Data.Role)
			{
				pc.Data.Role = Object.Instantiate(GameData.Instance.DefaultRole);
				pc.Data.Role.Initialize(pc);
			}
			yield return null;
		}
		else
		{
			pc.StartCoroutine(pc.ClientInitialize());
		}
		pc.MyPhysics.SetBodyType(pc.BodyType);
		if (pc.isNew)
		{
			pc.isNew = false;
			pc.StartCoroutine(pc.MyPhysics.CoSpawnPlayer(LobbyBehaviour.Instance));
		}
		if (PlayerControl.LocalPlayer == pc)
		{
			pc.clickKillCollider.enabled = false;
		}
		CustomRpcManager.CustomSpawnHandshake(pc);
    }
}