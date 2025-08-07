using System.Collections.Generic;
using BetterVanilla.Components;
using BetterVanilla.Options;
using UnityEngine;

namespace BetterVanilla.Core;

public static class LocalConditions
{

    public static bool ShouldAutoPlayAgain()
    {
        return LocalOptions.Default.AutoPlayAgain.Value;
    }

    public static bool ShouldShowRolesAndTasks()
    {
        return LocalOptions.Default.DisplayRolesAndTasksAfterDeath.Value;
    }

    public static bool ShouldShowRolesAndTasks(PlayerControl playerControl)
    {
        return ShouldShowRolesAndTasks() && IsGameStarted() && (PlayerControl.LocalPlayer == playerControl || AmDead());
    }

    public static bool AmDead()
    {
        if (LocalOptions.Default.DisableAmDeadCheck.IsNotAllowed() || !LocalOptions.Default.DisableAmDeadCheck.Value)
        {
            return PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data && PlayerControl.LocalPlayer.Data.IsDead;
        }
        return true;
    }

    public static bool AmImpostor()
    {
        if (LocalOptions.Default.DisableAmImpostorCheck.IsNotAllowed() || !LocalOptions.Default.DisableAmImpostorCheck.Value)
        {
            return PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data && PlayerControl.LocalPlayer.Data.Role && PlayerControl.LocalPlayer.Data.Role.IsImpostor;
        }
        return false;
    }

    public static bool AmAlive() => !AmDead();

    public static bool IsGameStarted()
    {
        return AmongUsClient.Instance && (AmongUsClient.Instance.IsGameStarted || TutorialManager.InstanceExists);
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

    public static bool IsForcedTeamAssignmentAllowed()
    {
        return LocalOptions.Default.ForcedTeamAssignment.IsAllowed();
    }

    public static bool CanZoom()
    {
        return BetterVanillaManager.Instance.ZoomBehaviour != null && AmDead() && !AmImpostor();
    }

    public static bool ShouldDisableGameStartRequirement()
    {
        return !LocalOptions.Default.DisableGameStartRequirement.IsNotAllowed() && LocalOptions.Default.DisableGameStartRequirement.Value;
    }

    public static bool ShouldDisableGameEndRequirement()
    {
        return !LocalOptions.Default.DisableEndGameChecks.IsNotAllowed() && LocalOptions.Default.DisableEndGameChecks.Value;
    }

    public static bool ShouldUnlockModdedCosmetics()
    {
        return !LocalOptions.Default.AllowModdedCosmetics.IsNotAllowed() && LocalOptions.Default.AllowModdedCosmetics.Value;
    }

    public static bool ShouldRevealVentPositionsInMap()
    {
        return LocalOptions.Default.DisplayVentsInMap.Value;
    }
}