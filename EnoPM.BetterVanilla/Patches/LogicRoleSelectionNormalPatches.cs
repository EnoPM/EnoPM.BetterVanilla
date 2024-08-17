using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(LogicRoleSelectionNormal))]
internal static class LogicRoleSelectionNormalPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(LogicRoleSelectionNormal.AssignRolesForTeam))]
    private static bool AssignRolesForTeamPrefix(LogicRoleSelectionNormal __instance, Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> players,
        IGameOptions opts,
        RoleTeamTypes team,
        int teamMax,
        RoleTypes? defaultRole)
    {
        var rolesAssigned = 0;
        var allRoles = DestroyableSingleton<RoleManager>.Instance.AllRoles
            .Where(x => x.TeamType == team && !RoleManager.IsGhostRole(x.Role))
            .ToList();

        var roleList = new Il2CppSystem.Collections.Generic.List<RoleTypes>();
        var roleOptions = opts.RoleOptions;

        var allRoleAssignmentData = allRoles.Where(x => roleOptions.GetChancePerGame(x.Role) == 100)
            .Select(x => new RoleManager.RoleAssignmentData(x, roleOptions.GetNumPerGame(x.Role), 100));

        foreach (var roleAssignmentData in allRoleAssignmentData)
        {
            while (roleAssignmentData.Count-- > 0)
            {
                roleList.Add(roleAssignmentData.Role.Role);
            }
        }
        AssignRolesFromList(players, teamMax, roleList, ref rolesAssigned);

        var availableRoles = new List<RoleManager.RoleAssignmentData>();
        foreach (var role in allRoles)
        {
            var chance = roleOptions.GetChancePerGame(role.Role);
            if (chance is <= 0 or >= 100) continue;
            availableRoles.Add(new RoleManager.RoleAssignmentData(role, roleOptions.GetNumPerGame(role.Role), chance));
        }
        roleList.Clear();

        foreach (var roleAssignmentData in availableRoles)
        {
            for (var i = 0; i < roleAssignmentData.Count; ++i)
            {
                if (HashRandom.Next(101) >= roleAssignmentData.Chance) continue;
                roleList.Add(roleAssignmentData.Role.Role);
            }
        }

        AssignRolesFromList(players, teamMax, roleList, ref rolesAssigned);

        if (!defaultRole.HasValue)
        {
            return false;
        }

        while (roleList.Count < players.Count && roleList.Count + rolesAssigned < teamMax)
        {
            roleList.Add(defaultRole.Value);
        }

        AssignRolesFromList(players, teamMax, roleList, ref rolesAssigned);

        return false;
    }

    private static void AssignRolesFromList(
        Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> players,
        int teamMax,
        Il2CppSystem.Collections.Generic.List<RoleTypes> roleList,
        ref int rolesAssigned)
    {
        while (roleList.Count > 0 && players.Count > 0 && rolesAssigned < teamMax)
        {
            var index1 = HashRandom.FastNext(roleList.Count);
            var role = roleList._items[index1];
            roleList.RemoveAt(index1);
            var index2 = HashRandom.FastNext(players.Count);
            players._items[index2].Object.RpcSetRole(role);
            players.RemoveAt(index2);
            ++rolesAssigned;
        }
    }
}