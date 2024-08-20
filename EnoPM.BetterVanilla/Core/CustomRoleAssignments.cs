using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using EnoPM.BetterVanilla.Core.Data;
using EnoPM.BetterVanilla.Core.Extensions;
using MonoMod.Utils;
using UnityEngine;

namespace EnoPM.BetterVanilla.Core;

public class CustomRoleAssignments
{
    private const int TicketsPerPlayer = 20;
    private const int TicketsPenaltyForNonPreferredTeam = 15;

    private readonly IGameOptions _currentOptions;
    private readonly int _numImpostors;
    private readonly List<PlayerControl> _allPlayers = [];
    private readonly List<PlayerControl> _remainingPlayers = [];
    private readonly Dictionary<byte, SettingTeamPreferences> _playerPreferences = [];
    private readonly Dictionary<byte, SettingTeamPreferences> _playerForcedAssignments = [];
    private readonly List<RoleTypes> _allCrewmateRoles = [];
    private readonly List<RoleTypes> _allImpostorRoles = [];

    public CustomRoleAssignments()
    {
        foreach (var playerControl in PlayerControl.AllPlayerControls)
        {
            if (!playerControl) continue;
            if (!playerControl.Data) continue;
            if (playerControl.Data.Disconnected || playerControl.Data.IsDead) continue;
            _allPlayers.Add(playerControl);
        }

        _currentOptions = GameOptionsManager.Instance.CurrentGameOptions;
        _numImpostors = GameOptionsManager.Instance.CurrentGameOptions.GetAdjustedNumImpostors(_allPlayers.Count);
        _remainingPlayers.AddRange(_allPlayers);
        SetupRoles();
    }

    public void SetTeamPreference(byte playerId, SettingTeamPreferences preference)
    {
        _playerPreferences[playerId] = preference;
    }

    public void SetTeamPreferences(Dictionary<byte, SettingTeamPreferences> preferences)
    {
        foreach (var preference in preferences)
        {
            SetTeamPreference(preference.Key, preference.Value);
        }
    }

    public void SetForcedAssignment(byte playerId, SettingTeamPreferences preference)
    {
        _playerForcedAssignments[playerId] = preference;
    }

    public void SetForcedAssignments(Dictionary<byte, SettingTeamPreferences> preferences)
    {
        foreach (var preference in preferences)
        {
            SetForcedAssignment(preference.Key, preference.Value);
        }
    }

    private void SetupRoles()
    {
        var roleOptions = _currentOptions.RoleOptions;

        foreach (var roleBehaviour in RoleManager.Instance.AllRoles)
        {
            var teamType = roleBehaviour.TeamType;
            var roleType = roleBehaviour.Role;
            var amount = roleOptions.GetNumPerGame(roleType);
            var chance = roleOptions.GetChancePerGame(roleType);
            if (chance == 0 || amount == 0) continue;
            var hasChanceRate = chance < 100;

            for (var i = 0; i < amount; i++)
            {
                if (!hasChanceRate || HashRandom.Next(101) < chance)
                {
                    if (teamType == RoleTeamTypes.Crewmate)
                    {
                        _allCrewmateRoles.Add(roleType);
                    }
                    else if (teamType == RoleTeamTypes.Impostor)
                    {
                        _allImpostorRoles.Add(roleType);
                    }
                }
            }
        }
    }

    private List<byte> CreateDraw(RoleTeamTypes team, bool ignorePlayerPreferences = false)
    {
        var draw = new List<byte>();
        var teamPreference = ConvertRoleTeamTypeToTeamPreference(team);
        var oppositeTeamPreference = GetOpposite(teamPreference);
        foreach (var player in _remainingPlayers)
        {
            var forcedAssignation = GetForcedAssignmentForPlayer(player);
            var playerId = player.PlayerId;
            var tickets = TicketsPerPlayer;
            var playerPreference = GetPreferenceForPlayer(player);
            if (!ignorePlayerPreferences && playerPreference != SettingTeamPreferences.Both && playerPreference != teamPreference)
            {
                tickets -= TicketsPenaltyForNonPreferredTeam;
            }

            if (forcedAssignation == SettingTeamPreferences.Both || forcedAssignation != oppositeTeamPreference)
            {
                for (var i = 0; i < tickets; i++)
                {
                    draw.Add(playerId);
                }
            }
        }

        return draw;
    }

    private SettingTeamPreferences GetPreferenceForPlayer(PlayerControl player) => _playerPreferences.GetValueOrDefault(player.PlayerId, SettingTeamPreferences.Both);
    private SettingTeamPreferences GetForcedAssignmentForPlayer(PlayerControl player) => _playerForcedAssignments.GetValueOrDefault(player.PlayerId, SettingTeamPreferences.Both);

    private static SettingTeamPreferences ConvertRoleTeamTypeToTeamPreference(RoleTeamTypes teamType) => teamType switch
    {
        RoleTeamTypes.Crewmate => SettingTeamPreferences.Crewmate,
        RoleTeamTypes.Impostor => SettingTeamPreferences.Impostor,
        _ => throw new ArgumentOutOfRangeException(nameof(teamType), $"Unable to find Preference correspondence for {teamType.ToString()}")
    };

    private static SettingTeamPreferences? GetOpposite(SettingTeamPreferences preference) => preference switch
    {
        SettingTeamPreferences.Crewmate => SettingTeamPreferences.Impostor,
        SettingTeamPreferences.Impostor => SettingTeamPreferences.Crewmate,
        _ => null
    };

    private List<PlayerControl> PickRandomPlayersFromDraw(List<byte> draw, int amount)
    {
        var result = new List<PlayerControl>();

        for (var i = 0; i < amount; i++)
        {
            if (draw.Count == 0)
            {
                return result;
                // throw new ArgumentException($"No enough tickets to pick {amount} players", nameof(draw));
            }

            var playerId = draw.PickOneRandom();
            var player = _remainingPlayers.First(x => x.PlayerId == playerId);
            result.Add(player);
            _remainingPlayers.Remove(player);

            while (draw.Contains(playerId))
            {
                draw.Remove(playerId);
            }
        }

        return result;
    }

    private List<PlayerControl> GetTeam(RoleTeamTypes teamType, int teamSize)
    {
        var draw = CreateDraw(teamType, !ModSettings.Host.TeamPreferencesAllowed);
        var result = PickRandomPlayersFromDraw(draw, teamSize);
        return result;
    }

    private static Dictionary<PlayerControl, RoleTypes> GetRolesAssignation(List<PlayerControl> players, List<RoleTypes> roles, RoleTypes defaultRole)
    {
        var result = new Dictionary<PlayerControl, RoleTypes>();
        foreach (var player in players)
        {
            var roleType = roles.Count > 0 ? roles.PickOneRandom() : defaultRole;
            result.Add(player, roleType);
        }

        return result;
    }

    public void StartAssignation()
    {
        var impostorsCount = Mathf.Max(1, _numImpostors);
        var impostorTeam = GetTeam(RoleTeamTypes.Impostor, impostorsCount);

        var crewmatesCount = _remainingPlayers.Count;
        var crewmateTeam = GetTeam(RoleTeamTypes.Crewmate, crewmatesCount);
        
        var playerRoles = new Dictionary<PlayerControl, RoleTypes>();
        playerRoles.AddRange(GetRolesAssignation(impostorTeam, _allImpostorRoles, RoleTypes.Impostor));
        playerRoles.AddRange(GetRolesAssignation(crewmateTeam, _allCrewmateRoles, RoleTypes.Crewmate));

        SendRolesAssignation(playerRoles);
    }

    private static void SendRolesAssignation(Dictionary<PlayerControl, RoleTypes> playerRoles)
    {
        foreach (var (player, roleType) in playerRoles)
        {
            player.RpcSetRole(roleType);
        }
    }
}