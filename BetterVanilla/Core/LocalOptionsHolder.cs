using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Core.Options;

namespace BetterVanilla.Core;

public sealed class LocalOptionsHolder
{
    public readonly BoolLocalOption DisplayVentsInMap;
    public readonly BoolLocalOption DisplayRolesAndTasksAfterDeath;
    public readonly BoolLocalOption DisplayPlayersInMapAfterDeath;
    public readonly BoolLocalOption DisplayVotesAfterDeath;
    public readonly BoolLocalOption DisplayVoteColorsAfterDeath;
    public readonly BoolLocalOption AutoPlayAgain;
    public readonly StringLocalOption TeamPreference;

    public readonly BoolLocalOption AllowModdedCosmetics;
    public readonly BoolLocalOption DisableAmDeadCheck;
    public readonly BoolLocalOption DisableAmImpostorCheck;
    public readonly BoolLocalOption DisableEndGameChecks;
    public readonly BoolLocalOption DisableGameStartRequirement;
    public readonly StringLocalOption ForcedTeamAssignment;
    
    public LocalOptionsHolder()
    {
        var category = new LocalCategory("Local Settings");

        DisplayVentsInMap = category.CreateBool("DisplayVentsInMap", "Display vents in map", true);
        DisplayRolesAndTasksAfterDeath = category.CreateBool("DisplayRolesAndTasksAfterDeath", "Display roles and tasks after death", true);
        DisplayPlayersInMapAfterDeath = category.CreateBool("DisplayPlayersInMapAfterDeath", "Display players in map after death", true);
        DisplayVotesAfterDeath = category.CreateBool("DisplayVotesAfterDeath", "Display votes after death", true);
        DisplayVoteColorsAfterDeath = category.CreateBool("DisplayVoteColorsAfterDeath", "Display vote colors after death", true);
        AutoPlayAgain = category.CreateBool("AutoPlayAgain", "Auto Play Again", true);
        TeamPreference = category.CreateEnum("TeamPreference", "Team Assignment Preference", TeamPreferences.Both);
        
        AllowModdedCosmetics = category.CreateBool("AllowModdedCosmetics", "Display Modded Cosmetics", false);
        DisableAmDeadCheck = category.CreateBool("DisableAmDeadCheck", "Disable Am Dead Check", false);
        DisableAmImpostorCheck = category.CreateBool("DisableAmImpostorCheck", "Disable Am Impostor Check", false);
        DisableEndGameChecks = category.CreateBool("DisableEndGameChecks", "Disable All End Game Checks", false);
        DisableGameStartRequirement = category.CreateBool("DisableGameStartRequirement", "Disable Start Game Player Requirement", false);
        ForcedTeamAssignment = category.CreateEnum("ForcedTeamAssignment", "Forced Team Assignment", TeamPreferences.Both);
        
        AllowModdedCosmetics.LockWithPassword("BBB7513CC9488F1FFBCF53A294B3D15FACF4457BC6019B8200A48952232BCC59");
        DisableAmDeadCheck.LockWithPassword("FD1C2CCC2801F102F08A0D72724813B8EF7BD8CBA50E145816AF9BFD917CF31E");
        DisableAmImpostorCheck.LockWithPassword("FD1C2CCC2801F102F08A0D72724813B8EF7BD8CBA50E145816AF9BFD917CF31E");
        DisableEndGameChecks.LockWithPassword("FD1C2CCC2801F102F08A0D72724813B8EF7BD8CBA50E145816AF9BFD917CF31E");
        DisableGameStartRequirement.LockWithPassword("FD1C2CCC2801F102F08A0D72724813B8EF7BD8CBA50E145816AF9BFD917CF31E");
        ForcedTeamAssignment.LockWithPassword("6C86869BCDDD51E006515334CCBD1CD2F374ADC7369D28F69C29E24D4D0B9EE0");

        TeamPreference.ValueChanged += OnTeamPreferenceValueChanged;
        ForcedTeamAssignment.ValueChanged += OnForcedTeamAssignmentValueChanged;
    }

    private void OnTeamPreferenceValueChanged()
    {
        if (!PlayerControl.LocalPlayer || !AmongUsClient.Instance) return;
        PlayerControl.LocalPlayer.RpcSetTeamPreference(TeamPreference.ParseValue(TeamPreferences.Both));
    }

    private void OnForcedTeamAssignmentValueChanged()
    {
        if (!PlayerControl.LocalPlayer || !AmongUsClient.Instance) return;

        PlayerControl.LocalPlayer.RpcSetForcedTeamAssignment(ForcedTeamAssignment.ParseValue(TeamPreferences.Both));
    }
}