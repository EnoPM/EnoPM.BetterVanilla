using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
using BetterVanilla.Core.Helpers;
using BetterVanilla.Options;
using InnerNet;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class PlayerControlExtensions
{
    public static void ReportPlayer(this PlayerControl player, ReportReasons reason = ReportReasons.None)
    {
        if (!player) return;
        var client = player.GetClient();
        if (client == null || client.HasBeenReported) return;
        AmongUsClient.Instance.ReportPlayer(client.Id, reason);
    }

    private static ClientData? GetClient(this PlayerControl player)
    {
        try
        {
            return AmongUsClient.Instance.allClients.ToArray().FirstOrDefault(cd => cd.Character.PlayerId == player.PlayerId);
        }
        catch
        {
            return null;
        }
    }

    public static List<NormalPlayerTask> GetRemainingTasks(this PlayerControl player)
    {
        if (!player || !player.Data || player.Data.Tasks == null || player.myTasks == null)
        {
            return [];
        }
        var results = new List<NormalPlayerTask>();
        foreach (var playerTask in player.myTasks)
        {
            if (!playerTask) continue;
            if (playerTask.IsComplete) continue;
            var normalPlayerTask = playerTask.TryCast<NormalPlayerTask>();
            if (normalPlayerTask != null)
            {
                if (normalPlayerTask.IsComplete) continue;
                results.Add(normalPlayerTask);
            }
        }
        return results;
    }

    public static IEnumerator CoBetterStart(this PlayerControl pc)
    {
        yield return CoroutineUtils.CoAssertWithTimeout(() => pc.PlayerId != byte.MaxValue, () => { AmongUsClient.Instance.EnqueueDisconnect(DisconnectReasons.Error, "Timeout while waiting for player ID assignment"); }, 30f);

        yield return CoroutineUtils.CoAssertWithTimeout(() => GameManager.Instance != null && GameData.Instance != null && pc.Data != null,
            () => { AmongUsClient.Instance.EnqueueDisconnect(DisconnectReasons.Error, "Timeout while waiting for player data containers"); }, 30f);
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
            while (mainCamera == null)
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
            if (HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat).BlocksVisors)
            {
                DataManager.Player.Customization.Visor = "visor_EmptyVisor";
            }
            pc.RpcSetVisor(DataManager.Player.Customization.Visor);
            pc.RpcSetNamePlate(DataManager.Player.Customization.NamePlate);
            var playerLevel = DataManager.Player.Stats.Level;
            SponsorOptions.Default.LevelOverride.MaxValue = DataManager.Player.Stats.Level + 1;
            pc.RpcSetLevel(SponsorOptions.Default.LevelOverride.IsAllowed() ? (uint)Mathf.RoundToInt(SponsorOptions.Default.LevelOverride.Value - 1f) : playerLevel);
            pc.CustomOwnerSpawnHandshake();
            yield return null;
        }
        else
        {
            pc.StartCoroutine(pc.ClientInitialize());
        }
        if (pc.Data.Role == null)
        {
            pc.Data.Role = Object.Instantiate(GameData.Instance.DefaultRole);
            pc.Data.Role.Initialize(pc);
        }
        pc.MyPhysics.SetBodyType(pc.BodyType);
        if (pc.isNew)
        {
            pc.isNew = false;
            pc.StartCoroutine(pc.MyPhysics.CoSpawnPlayer());
        }
        if (PlayerControl.LocalPlayer == pc)
        {
            pc.clickKillCollider.enabled = false;
        }
        pc.CustomSpawnHandshake();
    }

    public static void RpcHidePet(this PlayerControl pc)
    {
        if (pc == null || pc.Data == null) return;
        var currentPet = pc.cosmetics?.currentPet?.Data?.ProductId;
        if (currentPet == null || currentPet == PetData.EmptyId) return;
        Ls.LogMessage($"Hiding pet for {pc.Data.PlayerName} : {currentPet}");
        pc.RpcSetPet(PetData.EmptyId);
    }

    public static void SetMovement(this PlayerControl source, bool canMove)
    {
        if (!source) return;
        source.moveable = canMove;
        source.MyPhysics.ResetMoveState(false);
        source.NetTransform.enabled = canMove;
        source.MyPhysics.enabled = canMove;
        source.NetTransform.Halt();
    }

    private static void CustomOwnerSpawnHandshake(this PlayerControl pc)
    {
        var player = pc.gameObject.GetComponent<BetterPlayerControl>();
        if (player == null)
        {
            Ls.LogError($"Unable to start {nameof(CustomOwnerSpawnHandshake)} because it doesn't have a {nameof(BetterPlayerControl)}");
            return;
        }

        player.StartCoroutine(player.CoOwnerSpawnHandshake());
    }

    private static void CustomSpawnHandshake(this PlayerControl _)
    {
        if (BetterPlayerControl.LocalPlayer == null) return;
        BetterPlayerControl.LocalPlayer.StartCoroutine(BetterPlayerControl.CoSpawnHandshake());
    }
}