using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
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

    private LocalOptions() : base("local")
    {
        TeamPreference.ValueChanged += OnTeamPreferenceValueChanged;
    }
    
    private void OnTeamPreferenceValueChanged()
    {
        if (!PlayerControl.LocalPlayer || !AmongUsClient.Instance) return;
        PlayerControl.LocalPlayer.RpcSetTeamPreference(TeamPreference.ParseValue(TeamPreferences.Both));
    }
}