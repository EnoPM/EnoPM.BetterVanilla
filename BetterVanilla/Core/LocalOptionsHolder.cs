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

    public readonly BoolLocalOption AutoOpenDoors;
    public readonly StringLocalOption DoorSwitchInterval;
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
        
        AutoOpenDoors = category.CreateBool("AutoOpenDoors", "Auto open doors", false);
        DoorSwitchInterval = category.CreateEnum("DoorSwitchInterval", "Door Switch Interval", DoorOpenDelay.Ms5);
        AllowModdedCosmetics = category.CreateBool("AllowModdedCosmetics", "Display Modded Cosmetics", false);
        DisableAmDeadCheck = category.CreateBool("DisableAmDeadCheck", "Disable Am Dead Check", false);
        DisableAmImpostorCheck = category.CreateBool("DisableAmImpostorCheck", "Disable Am Impostor Check", false);
        DisableEndGameChecks = category.CreateBool("DisableEndGameChecks", "Disable All End Game Checks", false);
        DisableGameStartRequirement = category.CreateBool("DisableGameStartRequirement", "Disable Start Game Player Requirement", false);
        ForcedTeamAssignment = category.CreateEnum("ForcedTeamAssignment", "Forced Team Assignment", TeamPreferences.Both);
        
        AllowModdedCosmetics.LockWithPassword("A617504DA5A04943DE0CF0C85FF2DA7B7C387C6B6D2950E1DF8CB8D0E4D69FCE");
        DisableAmDeadCheck.LockWithPassword("A21B151BDA9AD098622868D3B7EBB5B0BB819EBD7C25B6504BDAAE9FF1FF8C0F");
        DisableAmImpostorCheck.LockWithPassword("B640EC50173A2DAC2056F2926027D3BBDDEFB9A1468627A36164D4101519DDAA");
        DisableEndGameChecks.LockWithPassword("81133C46C6DAACD12259989C7D8D385BE47AF65385F9C3D5BFAFEDD2A5C44FD0");
        DisableGameStartRequirement.LockWithPassword("98F47935FFDEEDC0F716F87A5C332AA77222DF4EAF4FF61E1989EE95B1671B77");
        ForcedTeamAssignment.LockWithPassword("FE3C8AB240748CE91A143E7E61CA6C3D0996D55C23F8F3A7BC42A47BB26B7514");

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