using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Components;
using BetterVanilla.Core.Data;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Core;

public static class LocalConditions
{
    public static bool ShouldAutoPlayAgain()
    {
        return LocalOptions.Default.AutoPlayAgain.Value;
    }

    public static bool ShouldShowRolesAndTasks(PlayerControl playerControl)
    {
        return playerControl != null
               && LocalOptions.Default.DisplayRolesAndTasksAfterDeath.Value
               && IsGameStarted()
               && (PlayerControl.LocalPlayer == playerControl || AmDead());
    }

    public static bool AmDead()
    {
        if (FeatureOptions.Default.DisableAmDeadCheck.IsNotAllowed() || !FeatureOptions.Default.DisableAmDeadCheck.Value)
        {
            return PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data && PlayerControl.LocalPlayer.Data.IsDead;
        }
        return true;
    }

    private static bool AmImpostor()
    {
        if (FeatureOptions.Default.DisableAmImpostorCheck.IsNotAllowed() || !FeatureOptions.Default.DisableAmImpostorCheck.Value)
        {
            return PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data && PlayerControl.LocalPlayer.Data.Role && PlayerControl.LocalPlayer.Data.Role.IsImpostor;
        }
        return false;
    }

    public static bool AmAlive() => !AmDead();

    public static bool AmSponsor()
    {
        if (FeatureCodeBehaviour.Instance == null || FeatureCodeBehaviour.Instance.Registry == null || !EOSManager.InstanceExists || string.IsNullOrEmpty(EOSManager.Instance.FriendCode))
        {
            return false;
        }
        return FeatureCodeBehaviour.Instance.Registry.ContributorFriendCodes.Contains(EOSManager.Instance.FriendCode);
    }

    public static bool IsGameStarted()
    {
        return AmongUsClient.Instance != null && (AmongUsClient.Instance.IsGameStarted || TutorialManager.InstanceExists);
    }

    public static bool IsIncrementMultiplierKeyPressed()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private static readonly List<MapOptions.Modes> AllowedRevealMapModes = [MapOptions.Modes.Normal, MapOptions.Modes.Sabotage];

    public static bool ShouldRevealPlayerPositionsInMap(MapOptions.Modes currentMapMode)
    {
        return AllowedRevealMapModes.Contains(currentMapMode) && LocalOptions.Default.DisplayPlayersInMapAfterDeath.Value && !MeetingHud.Instance && AmDead() && !AmImpostor();
    }

    public static bool ShouldRevealVotes()
    {
        return LocalOptions.Default.DisplayVotesAfterDeath.Value && AmDead();
    }

    public static bool ShouldRevealVoteColors()
    {
        return LocalOptions.Default.DisplayVoteColorsAfterDeath.Value && AmDead();
    }

    public static bool CanZoom()
    {
        return BetterVanillaManager.Instance.ZoomBehaviour != null && AmDead() && !AmImpostor() && IsGameStarted();
    }

    public static bool ShouldDisableGameStartRequirement()
    {
        return FeatureOptions.Default.DisableGameStartRequirement.IsAllowed() && FeatureOptions.Default.DisableGameStartRequirement.Value;
    }

    public static bool ShouldDisableGameEndRequirement()
    {
        return FeatureOptions.Default.DisableEndGameChecks.IsAllowed() && FeatureOptions.Default.DisableEndGameChecks.Value;
    }

    public static bool ShouldUnlockModdedCosmetics()
    {
        return FeatureOptions.Default.AllowModdedCosmetics.IsAllowed() && FeatureOptions.Default.AllowModdedCosmetics.Value;
    }

    public static bool ShouldRevealVentPositionsInMap()
    {
        return LocalOptions.Default.DisplayVentsInMap.Value;
    }

    public static bool CanCompleteAutoTasks()
    {
        return IsGameStarted() && AmDead() && !AmImpostor();
    }

    public static bool AmHost()
    {
        return AmongUsClient.Instance != null && AmongUsClient.Instance.AmHost;
    }

    public static bool IsBetterVanillaHost()
    {
        if (AmHost()) return true;
        if (AmongUsClient.Instance == null) return false;
        var hostId = AmongUsClient.Instance.HostId;
        var player = BetterVanillaManager.Instance.GetPlayerByOwnerId(hostId);
        return player?.Handshake != null;
    }

    public static bool IsAllPlayersUsingBetterVanilla()
    {
        var localVersion = BetterVanillaHandshake.Local.Version;
        return BetterVanillaManager.Instance.AllPlayers.All(player => player.Handshake != null && player.Handshake.Version == localVersion);
    }
    
    public static bool IsBetterPolusEnabled() => HostOptions.Default.BetterPolus.IsAllowed() && HostOptions.Default.BetterPolus.Value;

    public static bool ShouldAnonymizePlayers()
    {
        if (!HostOptions.Default.AnonymizePlayersOnCamerasDuringLights.IsNotAllowed() || !HostOptions.Default.AnonymizePlayersOnCamerasDuringLights.Value)
        {
            return false;
        }
        return PlayerControl.LocalPlayer != null && !PlayerControl.LocalPlayer.Data.Role.IsImpostor && PlayerControl.LocalPlayer.myTasks.ToArray().Any(x => x.TaskType == TaskTypes.FixLights);
    }
}