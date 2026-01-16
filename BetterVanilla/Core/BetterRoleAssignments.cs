using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options;
using MonoMod.Utils;
using UnityEngine;

namespace BetterVanilla.Core;

public sealed class BetterRoleAssignments
{
    private const int BaseWeight = 100;
    private const int PenaltyForNonPreferredTeam = 15;
    private const int BonusForForcedAssignment = 200;

    private readonly IGameOptions _currentOptions;
    private readonly int _numImpostors;
    private readonly List<PlayerControl> _allPlayers = [];
    private readonly List<PlayerControl> _remainingPlayers = [];
    private readonly Dictionary<int, TeamPreferences> _playerPreferences = [];
    private readonly Dictionary<int, TeamPreferences> _playerForcedAssignments = [];
    private readonly List<RoleTypes> _allCrewmateRoles = [];
    private readonly List<RoleTypes> _allImpostorRoles = [];

    public BetterRoleAssignments()
    {
        foreach (var playerControl in PlayerControl.AllPlayerControls)
        {
            if (playerControl == null || playerControl.Data == null) continue;
            if (playerControl.Data.Disconnected || playerControl.Data.IsDead) continue;
            _allPlayers.Add(playerControl);
        }

        _currentOptions = GameOptionsManager.Instance.CurrentGameOptions;
        _numImpostors = GameOptionsManager.Instance.CurrentGameOptions.GetAdjustedNumImpostors(_allPlayers.Count);
        _remainingPlayers.AddRange(_allPlayers);
        SetupRoles();
    }

    public void SetTeamPreference(int playerId, TeamPreferences preference)
    {
        _playerPreferences[playerId] = preference;
    }

    public void SetTeamPreferences(Dictionary<int, TeamPreferences> preferences)
    {
        foreach (var preference in preferences)
        {
            SetTeamPreference(preference.Key, preference.Value);
        }
    }

    public void SetForcedAssignment(int playerId, TeamPreferences preference)
    {
        _playerForcedAssignments[playerId] = preference;
    }

    public void SetForcedAssignments(Dictionary<int, TeamPreferences> preferences)
    {
        foreach (var preference in preferences)
        {
            SetForcedAssignment(preference.Key, preference.Value);
        }
    }

    private void SetupRoles()
    {
        var roleOptions = _currentOptions.RoleOptions;

        var allRoles = RoleManager.Instance.AllRoles
            .ToArray()
            .Where(x => !RoleManager.IsGhostRole(x.Role));

        foreach (var roleBehaviour in allRoles)
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

    private Dictionary<byte, int> CalculateWeights(RoleTeamTypes team, bool ignorePlayerPreferences = false)
    {
        var weights = new Dictionary<byte, int>();
        var teamPreference = ConvertRoleTeamTypeToTeamPreference(team);
        var oppositeTeamPreference = GetOpposite(teamPreference);

        foreach (var player in _remainingPlayers)
        {
            var forcedAssignment = GetForcedAssignmentForPlayer(player);
            var playerId = player.PlayerId;
            var playerPreference = GetPreferenceForPlayer(player);

            var weight = BaseWeight; // Poids de base

            // Ajuster le poids en fonction des préférences du joueur
            if (!ignorePlayerPreferences && playerPreference != TeamPreferences.Both &&
                playerPreference != teamPreference)
            {
                weight -= PenaltyForNonPreferredTeam; // Réduire le poids si la préférence est pour l'équipe opposée
            }

            // Si une assignation forcée est en place, ignorer les préférences et ajuster le poids
            if (forcedAssignment != TeamPreferences.Both)
            {
                if (forcedAssignment == oppositeTeamPreference)
                {
                    weight = 0; // Si l'assignation forcée est pour l'équipe opposée, ce joueur ne sera pas choisi pour cette équipe
                }
                else
                {
                    weight =
                        BonusForForcedAssignment; // Si l'assignation forcée est pour cette équipe, donner un poids très élevé
                }
            }

            if (weight > 0)
            {
                weights[playerId] = weight;
            }
        }

        return weights;
    }

    private List<PlayerControl> PickRandomPlayersBasedOnWeights(Dictionary<byte, int> weights, int amount)
    {
        var selectedPlayers = new List<PlayerControl>();
        var remainingWeights = weights.ToDictionary(entry => entry.Key, entry => entry.Value);

        for (var i = 0; i < amount; i++)
        {
            if (remainingWeights.Count == 0)
            {
                break; // Si plus de joueurs disponibles
            }

            // Calculer le total des poids restants
            int totalWeight = remainingWeights.Values.Sum();

            // Sélectionner un joueur en fonction de la distribution des poids
            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            byte selectedPlayerId = 0;

            foreach (var playerId in remainingWeights.Keys)
            {
                if (randomValue < remainingWeights[playerId])
                {
                    selectedPlayerId = playerId;
                    break;
                }

                randomValue -= remainingWeights[playerId];
            }

            var selectedPlayer = _remainingPlayers.First(x => x.PlayerId == selectedPlayerId);
            selectedPlayers.Add(selectedPlayer);
            _remainingPlayers.Remove(selectedPlayer);
            remainingWeights.Remove(selectedPlayerId); // Retirer le joueur sélectionné des poids restants
        }

        return selectedPlayers;
    }

    private List<PlayerControl> GetTeam(RoleTeamTypes teamType, int teamSize)
    {
        var weights = CalculateWeights(teamType, !HostOptions.Default.AllowTeamPreference.Value);
        var result = PickRandomPlayersBasedOnWeights(weights, teamSize);
        return result;
    }

    private static Dictionary<PlayerControl, RoleTypes> GetRolesAssignation(List<PlayerControl> players,
        List<RoleTypes> roles, RoleTypes defaultRole)
    {
        var result = new Dictionary<PlayerControl, RoleTypes>();
        foreach (var player in players)
        {
            var roleType = roles.Count > 0 ? roles.PickOneRandom() : defaultRole;
            result.Add(player, roleType);
        }

        return result;
    }

#if DEBUG
    private void DebugPreferences()
    {
        Ls.LogMessage($"**** Team Preferences ****");
        foreach (var preference in _playerPreferences)
        {
            var player = BetterVanillaManager.Instance.GetPlayerByOwnerId(preference.Key);
            var playerName = player ? player.Player.Data.PlayerName : "???";
            Ls.LogMessage($"{playerName} ({preference.Key}): {preference.Value.ToString()}");
        }
        
        Ls.LogMessage($"**** Forced Team Assignation ****");
        foreach (var preference in _playerForcedAssignments)
        {
            var player = BetterVanillaManager.Instance.GetPlayerByOwnerId(preference.Key);
            var playerName = player ? player.Player.Data.PlayerName : "???";
            Ls.LogMessage($"{playerName} ({preference.Key}): {preference.Value.ToString()}");
        }
    }
#endif

    public void StartAssignation()
    {
#if DEBUG
        DebugPreferences();
#endif
        var impostorCount = Mathf.Max(1, _numImpostors);
        var impostorTeam = GetTeam(RoleTeamTypes.Impostor, impostorCount);

        var crewmateCount = _remainingPlayers.Count;
        var crewmateTeam = GetTeam(RoleTeamTypes.Crewmate, crewmateCount);

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

    private TeamPreferences GetPreferenceForPlayer(PlayerControl player) =>
        _playerPreferences.GetValueOrDefault(player.OwnerId, TeamPreferences.Both);

    private TeamPreferences GetForcedAssignmentForPlayer(PlayerControl player) =>
        _playerForcedAssignments.GetValueOrDefault(player.OwnerId, TeamPreferences.Both);

    private static TeamPreferences ConvertRoleTeamTypeToTeamPreference(RoleTeamTypes teamType) => teamType switch
    {
        RoleTeamTypes.Crewmate => TeamPreferences.Crewmate,
        RoleTeamTypes.Impostor => TeamPreferences.Impostor,
        _ => throw new ArgumentOutOfRangeException(nameof(teamType),
            $"Unable to find Preference correspondence for {teamType.ToString()}")
    };

    private static TeamPreferences? GetOpposite(TeamPreferences preference) => preference switch
    {
        TeamPreferences.Crewmate => TeamPreferences.Impostor,
        TeamPreferences.Impostor => TeamPreferences.Crewmate,
        _ => null
    };
}