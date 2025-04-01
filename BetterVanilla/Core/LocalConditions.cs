using System.Collections.Generic;
using BetterVanilla.Components;
using UnityEngine;

namespace BetterVanilla.Core;

public static class LocalConditions
{
    public static LocalOptionsHolder Options => BetterVanillaManager.Instance.LocalOptions;

    public static bool ShouldAutoPlayAgain()
    {
        return Options.AutoPlayAgain.Value;
    }

    public static bool ShouldShowRolesAndTasks()
    {
        return Options.DisplayRolesAndTasksAfterDeath.Value;
    }

    public static bool ShouldShowRolesAndTasks(PlayerControl playerControl)
    {
        return ShouldShowRolesAndTasks() && IsGameStarted() && (PlayerControl.LocalPlayer == playerControl || AmDead());
    }

    public static bool AmDead()
    {
        if (Options.DisableAmDeadCheck.IsLocked() || !BetterVanillaManager.Instance.LocalOptions.DisableAmDeadCheck.Value)
        {
            return PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data && PlayerControl.LocalPlayer.Data.IsDead;
        }
        return true;
    }

    public static bool AmImpostor()
    {
        if (Options.DisableAmImpostorCheck.IsLocked() || !Options.DisableAmImpostorCheck.Value)
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
        return AllowedRevealMapModes.Contains(currentMapMode) && Options.DisplayPlayersInMapAfterDeath.Value && !MeetingHud.Instance && AmDead() && !AmImpostor();
    }

    public static bool ShouldRevealVotes()
    {
        return Options.DisplayVotesAfterDeath.Value && AmDead();
    }

    public static bool ShouldRevealVoteColors()
    {
        return Options.DisplayVoteColorsAfterDeath.Value && AmDead();
    }

    public static bool IsForcedTeamAssignmentAllowed()
    {
        return !Options.ForcedTeamAssignment.IsLocked();
    }

    public static bool CanZoom()
    {
        return BetterVanillaManager.Instance.ZoomBehaviour && AmDead() && !AmImpostor();
    }

    public static bool ShouldDisableGameStartRequirement()
    {
        return !Options.DisableGameStartRequirement.IsLocked() && Options.DisableGameStartRequirement.Value;
    }

    public static bool ShouldDisableGameEndRequirement()
    {
        return !Options.DisableEndGameChecks.IsLocked() && Options.DisableEndGameChecks.Value;
    }

    public static bool ShouldUnlockModdedCosmetics()
    {
        return !Options.AllowModdedCosmetics.IsLocked() && Options.AllowModdedCosmetics.Value;
    }

    public static bool ShouldRevealVentPositionsInMap()
    {
        return Options.DisplayVentsInMap.Value;
    }
}