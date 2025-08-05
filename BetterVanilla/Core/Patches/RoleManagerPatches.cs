using AmongUs.GameOptions;
using BetterVanilla.Components;
using BetterVanilla.Core.Data;
using BetterVanilla.Options;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(RoleManager))]
internal static class RoleManagerPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(RoleManager.SelectRoles))]
    private static bool SelectRolesPrefix(RoleManager __instance)
    {
        if (GameOptionsManager.Instance.currentGameMode != GameModes.Normal)
        {
            return true;
        }

        var manager = BetterVanillaManager.Instance;
        if (manager.HostOptions.AllowTeamPreference.GetBool())
        {
            manager.AllTeamPreferences[PlayerControl.LocalPlayer.OwnerId] = LocalOptions.Default.TeamPreference.ParseValue(TeamPreferences.Both);
        }
        if (LocalConditions.IsForcedTeamAssignmentAllowed())
        {
            manager.AllForcedTeamAssignments[PlayerControl.LocalPlayer.OwnerId] = LocalOptions.Default.ForcedTeamAssignment.ParseValue(TeamPreferences.Both);
        }
        
        var customAssignment = new BetterRoleAssignments();
        
        customAssignment.SetTeamPreferences(manager.AllTeamPreferences);
        customAssignment.SetForcedAssignments(manager.AllForcedTeamAssignments);
        customAssignment.StartAssignation();
        return false;
    }
}