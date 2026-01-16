using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using BetterVanilla.Options.Core.Local;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options;

public sealed class LocalOptions : AbstractSerializableOptionHolder
{
    public static readonly LocalOptions Default = new();
    
    [BoolOption(true)]
    [OptionName("Display vents in map")]
    public BoolLocalOption DisplayVentsInMap { get; set; } = null!;
    
    [BoolOption(true)]
    [OptionName("Display roles and tasks after death")]
    public BoolLocalOption DisplayRolesAndTasksAfterDeath { get; set; } = null!;
    
    [BoolOption(true)]
    [OptionName("Display players in map after death")]
    public BoolLocalOption DisplayPlayersInMapAfterDeath { get; set; } = null!;
    
    [BoolOption(true)]
    [OptionName("Display votes after death")]
    public BoolLocalOption DisplayVotesAfterDeath { get; set; } = null!;
    
    [BoolOption(true)]
    [OptionName("Display vote colors after death")]
    public BoolLocalOption DisplayVoteColorsAfterDeath { get; set; } = null!;
    
    [BoolOption(true)]
    [OptionName("Auto play again")]
    public BoolLocalOption AutoPlayAgain { get; set; } = null!;
    
    [EnumOption(TeamPreferences.Both)]
    [OptionName("Team assignment preference")]
    public EnumLocalOption TeamPreference { get; set; } = null!;
    
    [BoolOption(true)]
    [OptionName("Hide my pet after death")]
    public BoolLocalOption HideMyPetAfterDeath { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Auto open doors")]
    public BoolLocalOption AutoOpenDoors { get; set; } = null!;
    
    [ColorOption("#C22984")]
    [OptionName("Task text gradient start color")]
    public ColorLocalOption TaskStartColor { get; set; } = null!;
    
    [ColorOption("#56FFB0")]
    [OptionName("Task text gradient end color")]
    public ColorLocalOption TaskEndColor { get; set; } = null!;
    
    [BoolOption(true)]
    [OptionName("Display BetterVanilla version")]
    public BoolLocalOption DisplayBetterVanillaVersion { get; set; } = null!;

    private LocalOptions() : base("local")
    {
        TeamPreference.ValueChanged += OnTeamPreferenceValueChanged;
        
        TeamPreference.SetLockedText("Disabled by host");
        TeamPreference.SetIsLockedFunc(IsTeamPreferenceLocked);
        
        DisplayVotesAfterDeath.SetLockedText("Disabled by host");
        DisplayVotesAfterDeath.SetIsLockedFunc(IsDisplayVotesLocked);
        
        DisplayVoteColorsAfterDeath.SetLockedText("Disabled by host");
        DisplayVoteColorsAfterDeath.SetIsLockedFunc(IsDisplayVotesLocked);
        
        HideMyPetAfterDeath.SetLockedText("Forced by host");
        HideMyPetAfterDeath.SetIsLockedFunc(IsHidePetAfterDeathForced);
    }
    
    private void OnTeamPreferenceValueChanged()
    {
        if (BetterPlayerControl.LocalPlayer == null || AmongUsClient.Instance == null) return;
        BetterPlayerControl.LocalPlayer.RpcSetTeamPreference(TeamPreference.ParseValue(TeamPreferences.Both));
    }

    private bool IsTeamPreferenceLocked()
    {
        if (!LocalConditions.IsBetterVanillaHost())
        {
            TeamPreference.SetLockedText("Host does not use BetterVanilla");
            return true;
        }
        TeamPreference.SetLockedText("Not authorized by host");
        return !HostOptions.Default.AllowTeamPreference.Value;
    }
    
    private bool IsDisplayVotesLocked()
    {
        if (!LocalConditions.IsBetterVanillaHost())
        {
            DisplayVotesAfterDeath.SetLockedText("Host does not use BetterVanilla");
            DisplayVoteColorsAfterDeath.SetLockedText("Host does not use BetterVanilla");
            return true;
        }
        DisplayVotesAfterDeath.SetLockedText("Not authorized by host");
        DisplayVoteColorsAfterDeath.SetLockedText("Not authorized by host");
        return !HostOptions.Default.AllowDeadVoteDisplay.Value;
    }
    
    private static bool IsHidePetAfterDeathForced()
    {
        return LocalConditions.IsBetterVanillaHost() && HostOptions.Default.HideDeadPlayerPets.Value;
    }
}