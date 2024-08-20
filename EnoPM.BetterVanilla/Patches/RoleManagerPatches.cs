using System.Collections.Generic;
using AmongUs.GameOptions;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Core.Data;
using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(RoleManager))]
internal static class RoleManagerPatches
{
    internal static readonly Dictionary<byte, SettingTeamPreferences> PlayerForcedTeams = [];
    internal static readonly Dictionary<byte, SettingTeamPreferences> PlayerTeamPreferences = [];
    
    [HarmonyPrefix, HarmonyPatch(nameof(RoleManager.SelectRoles))]
    private static bool SelectRolesPrefix(RoleManager __instance)
    {
        if (GameOptionsManager.Instance.currentGameMode != GameModes.Normal)
        {
            return true;
        }

        PlayerTeamPreferences.TryAdd(PlayerControl.LocalPlayer.PlayerId, ModSettings.Local.TeamPreference);
        if (!ModSettings.Local.ForcedTeamAssignment.IsLocked())
        {
            PlayerForcedTeams.TryAdd(PlayerControl.LocalPlayer.PlayerId, ModSettings.Local.ForcedTeamAssignment);
        }
        
        var customAssignment = new CustomRoleAssignments();
        customAssignment.SetTeamPreferences(PlayerTeamPreferences);
        customAssignment.SetForcedAssignments(PlayerForcedTeams);
        PlayerTeamPreferences.Clear();
        customAssignment.StartAssignation();
        return false;
    }
}