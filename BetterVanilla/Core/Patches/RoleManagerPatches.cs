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
        Ls.LogMessage($"AllRoles: {__instance.AllRoles.Count}");
        
        if (GameOptionsManager.Instance.currentGameMode != GameModes.Normal)
        {
            return true;
        }

        var manager = BetterVanillaManager.Instance;
        if (HostOptions.Default.AllowTeamPreference.Value)
        {
            manager.AllTeamPreferences[PlayerControl.LocalPlayer.OwnerId] = LocalOptions.Default.TeamPreference.ParseValue(TeamPreferences.Both);
        }
        if (FeatureOptions.Default.ForcedTeamAssignment.IsAllowed())
        {
            manager.AllForcedTeamAssignments[PlayerControl.LocalPlayer.OwnerId] = FeatureOptions.Default.ForcedTeamAssignment.ParseValue(TeamPreferences.Both);
        }
        
        var customAssignment = new BetterRoleAssignments();
        
        customAssignment.SetTeamPreferences(manager.AllTeamPreferences);
        customAssignment.SetForcedAssignments(manager.AllForcedTeamAssignments);
        customAssignment.StartAssignation();
        return false;
    }
}