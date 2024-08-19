using System.Linq;
using AmongUs.GameOptions;
using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Core.Data;
using HarmonyLib;
using InnerNet;

namespace EnoPM.BetterVanilla.Patches;

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
        
        var customAssignment = new CustomRoleAssignments(new System.Collections.Generic.Dictionary<byte, SettingTeamPreferences> { {PlayerControl.LocalPlayer.PlayerId, SettingTeamPreferences.Crewmate} });
        customAssignment.StartAssignation();
        return false;
    }
}