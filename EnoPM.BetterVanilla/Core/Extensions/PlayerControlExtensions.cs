using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using InnerNet;
using TMPro;
using UnityEngine;

namespace EnoPM.BetterVanilla.Core.Extensions;

internal static class PlayerControlExtensions
{
    private static readonly Dictionary<int, TextMeshPro> ModdedTexts = new();

    private static TextMeshPro GetModdedText(this PlayerControl pc)
    {
        if (ModdedTexts.TryGetValue(pc.OwnerId, out var value) && value) return value;
        if (!pc.cosmetics) return null;
        ModdedTexts[pc.OwnerId] = UnityEngine.Object.Instantiate(pc.cosmetics.nameText, pc.cosmetics.nameText.transform.parent);
        var pos = ModdedTexts[pc.OwnerId].transform.localPosition;
        pos.y = 0.2f;
        ModdedTexts[pc.OwnerId].transform.localPosition = pos;
        ModdedTexts[pc.OwnerId].gameObject.SetActive(true);
        return ModdedTexts[pc.OwnerId];
    }

    internal static void ModdedFixedUpdate(this PlayerControl pc)
    {
        if (pc.Data == null || !pc.cosmetics) return;
        var tmp = pc.GetModdedText();
        if (!tmp) return;
        var isActive = Utils.AmDead || pc.AmOwner || !Utils.IsGameStarted;
        tmp.gameObject.SetActive(isActive);
        if (!isActive) return;
        var text = pc.Data.GenerateModdedText();
        if (tmp.text != text)
        {
            tmp.SetText(text);
        }
    }

    public static ClientData GetClient(this PlayerControl player)
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

    public static int GetClientId(this PlayerControl player)
    {
        return player?.GetClient()?.Id ?? -1;
    }

    public static string GetPlayerNameAndColor(this PlayerControl player)
    {
        if (!player || !player.Data) return string.Empty;
        try
        {
            return Utils.Cs(Palette.PlayerColors[player.CurrentOutfit.ColorId], player.Data.PlayerName);
        }
        catch
        {
            return player.Data.PlayerName;
        }
    }

    public static void Kick(this PlayerControl player, bool ban = false, string setReasonInfo = "")
    {
        if (Utils.IsHost(player) || AmongUsClient.Instance.AmHost) return;
        if (setReasonInfo != "")
        {
            player.RpcSetName(setReasonInfo + "<size=0%>");
        }
        AmongUsClient.Instance.KickPlayer(player.GetClientId(), ban);
    }

    public static int GetPlayerVentId(this PlayerControl player)
    {
        if (!player) return -1;
        if (!ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Ventilation, out var systemType)) return -1;

        var ventilationSystem = systemType.TryCast<VentilationSystem>();
        if (ventilationSystem == null)
        {
            return 0;
        }

        if (ventilationSystem.PlayersInsideVents.TryGetValue(player.PlayerId, out var result))
        {
            return result;
        }

        return -1;
    }
    
    public static Vector2 GetCustomPosition(this PlayerControl player)
    {
        return new Vector2(player.transform.position.x, player.transform.position.y);
    }
    
    public static bool IsAlive(this PlayerControl player)
    {
        return player && player.Data && !player.Data.IsDead;
    }
    
    public static bool IsInVent(this PlayerControl player)
    {
        if (!player || player.inVent || player.walkingToVent) return false;
        if (!player.MyPhysics)
        {
            return false;
        }
        var animations = player.MyPhysics.Animations;
        return animations && animations.IsPlayingEnterVentAnimation();
    }
    
    public static bool IsShapeShifting(this PlayerControl player)
    {
        return player && (player.shapeshiftTargetPlayerId >= 0 || player.shapeshifting);
    }
    
    public static bool IsInVanish(this PlayerControl player)
    {
        if (!player) return false;
        if (player.Data.Role is PhantomRole phantomRole)
        {
            return phantomRole && phantomRole.fading;
        }
        return false;
    }
    
    public static bool Is(this PlayerControl player, RoleTypes role)
    {
        if (!player || !player.Data) return false;
        return player.Data.RoleType == role;
    }
    
    public static bool IsGhostRole(this PlayerControl player)
    {
        return player.Is(RoleTypes.GuardianAngel);
    }

    public static bool IsImpostorTeam(this PlayerControl player)
    {
        return player.Is(RoleTypes.Impostor) || player.Is(RoleTypes.ImpostorGhost) || player.Is(RoleTypes.Shapeshifter) || player.Is(RoleTypes.Phantom);
    }
    
    public static bool IsImpostorTeammate(this PlayerControl player)
    {
        return player && PlayerControl.LocalPlayer && (player == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.IsImpostorTeam() || PlayerControl.LocalPlayer.IsImpostorTeam() && player.IsImpostorTeam());
    }
    
    public static bool IsCheater(this PlayerControl player)
    {
        return player && CheatsManager.IsCheating(player);
    }
    
    public static string GetPuId(this PlayerControl player)
    {
        if (!player || !player.Data || player.Data.Puid == null) return string.Empty;
        return player.Data.Puid;
    }

    public static void ReportPlayer(this PlayerControl player, ReportReasons reason = ReportReasons.None)
    {
        if (!player) return;
        if (player.GetClient().HasBeenReported) return;
        AmongUsClient.Instance.ReportPlayer(player.GetClientId(), reason);
    }
}