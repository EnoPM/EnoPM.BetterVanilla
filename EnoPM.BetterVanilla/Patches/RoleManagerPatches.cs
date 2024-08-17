using System.Linq;
using AmongUs.GameOptions;
using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Extensions;
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
        Plugin.Logger.LogMessage($"{nameof(SelectRolesPrefix)} invoked");
        CustomRoleAssignments.Start();
        return false;
        if (GameManager.Instance.LogicRoleSelection is not LogicRoleSelectionNormal)
        {
            return true;
        }
        Plugin.Logger.LogMessage($"{nameof(SelectRolesPrefix)} invoked");
        var clientDataList = new List<ClientData>();
        AmongUsClient.Instance.GetAllClients(clientDataList);
        var list = clientDataList._items.Where(x => x.Character && x.Character.Data && !x.Character.Data.Disconnected && !x.Character.Data.IsDead)
            .OrderBy(x => x.Id)
            .Select(x => x.Character.Data)
            .ToIl2CppList();

        foreach (var player in GameData.Instance.AllPlayers)
        {
            if (player.Object && player.Object.isDummy)
            {
                list.Add(player);
            }
        }
        var currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
        var adjustedNumImpostors = GameOptionsManager.Instance.CurrentGameOptions.GetAdjustedNumImpostors(list.Count);
        __instance.DebugRoleAssignments(list, ref adjustedNumImpostors);

        var defaultImpostorRole = new Il2CppSystem.Nullable<RoleTypes>(RoleTypes.Impostor);
        var defaultCrewmateRole = new Il2CppSystem.Nullable<RoleTypes>(RoleTypes.Crewmate);
        
        GameManager.Instance.LogicRoleSelection.AssignRolesForTeam(list, currentGameOptions, RoleTeamTypes.Impostor, adjustedNumImpostors, defaultImpostorRole);
        GameManager.Instance.LogicRoleSelection.AssignRolesForTeam(list, currentGameOptions, RoleTeamTypes.Crewmate, int.MaxValue, defaultCrewmateRole);
        
        return false;
    }
}