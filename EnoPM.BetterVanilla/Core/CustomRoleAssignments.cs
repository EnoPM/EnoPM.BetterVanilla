using System.Collections.Generic;
using System.Linq;

namespace EnoPM.BetterVanilla.Core;

public static class CustomRoleAssignments
{
    public static void Start()
    {
        var allPlayers = new List<PlayerControl>();
        foreach (var playerControl in PlayerControl.AllPlayerControls)
        {
            if (!playerControl) continue;
            if (!playerControl.Data) continue;
            if (playerControl.Data.Disconnected || playerControl.Data.IsDead) continue;
            allPlayers.Add(playerControl);
        }

        var currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
        var numImpostors = GameOptionsManager.Instance.CurrentGameOptions.GetAdjustedNumImpostors(allPlayers.Count);
        
        Plugin.Logger.LogMessage($"All players: {allPlayers.Count}");
        Plugin.Logger.LogMessage($"Number of impostors: {numImpostors}");
    }
    
    public sealed class PlayerTeamAssignationPreferences
    {
        public readonly List<RoleTeamTypes> TeamHistory = [];
        public readonly RoleTeamTypes? Preference = null;
    }
}