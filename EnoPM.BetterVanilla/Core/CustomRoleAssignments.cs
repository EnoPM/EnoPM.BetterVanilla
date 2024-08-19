using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using EnoPM.BetterVanilla.Core.Data;
using EnoPM.BetterVanilla.Core.Extensions;
using MonoMod.Utils;

namespace EnoPM.BetterVanilla.Core;

public class CustomRoleAssignments
{
    private const int TicketsPerPlayer = 20;
    private const int TicketsPenaltyForNonPreferredTeam = 15;
    
    private readonly IGameOptions _currentOptions;
    private readonly int _numImpostors;
    private readonly List<PlayerControl> _allPlayers = [];
    private readonly List<PlayerControl> _remainingPlayers = [];
    private readonly Dictionary<byte, SettingTeamPreferences> _playerPreferences;
    private readonly List<RoleTypes> _allCrewmateRoles = [];
    private readonly List<RoleTypes> _allImpostorRoles = [];
    
    public CustomRoleAssignments(Dictionary<byte, SettingTeamPreferences> playerPreferences = null)
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
        _playerPreferences = playerPreferences ?? [];
        _remainingPlayers.AddRange(_allPlayers);
        SetupRoles();
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
        var teamPreference = GetPreferenceCorrespondence(team);
        foreach (var player in _remainingPlayers)
        {
            var playerId = player.PlayerId;
            var tickets = TicketsPerPlayer;
            var playerPreference = GetPreferenceForPlayer(player);
            if (!ignorePlayerPreferences && playerPreference != SettingTeamPreferences.Both && playerPreference != teamPreference)
            {
                tickets -= TicketsPenaltyForNonPreferredTeam;
            }

            for (var i = 0; i < tickets; i++)
            {
                draw.Add(playerId);
            }
        }

        return draw;
    }

    private SettingTeamPreferences GetPreferenceForPlayer(PlayerControl player) => _playerPreferences.GetValueOrDefault(player.PlayerId, SettingTeamPreferences.Both);

    private static SettingTeamPreferences GetPreferenceCorrespondence(RoleTeamTypes teamType) => teamType switch
    {
        RoleTeamTypes.Crewmate => SettingTeamPreferences.Crewmate,
        RoleTeamTypes.Impostor => SettingTeamPreferences.Impostor,
        _ => throw new ArgumentOutOfRangeException(nameof(teamType), $"Unable to find Preference correspondence for {teamType.ToString()}")
    };

    private List<PlayerControl> PickRandomPlayersFromDraw(List<byte> draw, int amount)
    {
        var result = new List<PlayerControl>();

        for (var i = 0; i < amount; i++)
        {
            if (draw.Count == 0)
            {
                throw new ArgumentException($"No enough tickets to pick {amount} players", nameof(draw));
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
        return PickRandomPlayersFromDraw(CreateDraw(teamType), teamSize);
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
        var impostorsCount = _numImpostors;
        var impostorTeam = GetTeam(RoleTeamTypes.Impostor, impostorsCount);
        
        var crewmatesCount = _remainingPlayers.Count;
        var crewmateTeam = GetTeam(RoleTeamTypes.Crewmate, crewmatesCount);
        
        Plugin.Logger.LogMessage($"Impostor Team: {impostorsCount} {impostorTeam.Count}");
        Plugin.Logger.LogMessage($"Crewmate Team: {crewmatesCount} {crewmateTeam.Count}");
        
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