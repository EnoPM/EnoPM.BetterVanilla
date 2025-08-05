using BetterVanilla.Components;
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
    
    [BoolOption(false)]
    [OptionName("Display Modded Cosmetics")]
    public BoolLocalOption AllowModdedCosmetics { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Disable Am Dead Check")]
    public BoolLocalOption DisableAmDeadCheck { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Disable Am Impostor Check")]
    public BoolLocalOption DisableAmImpostorCheck { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Disable All End Game Checks")]
    public BoolLocalOption DisableEndGameChecks { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Disable Start Game Player Requirement")]
    public BoolLocalOption DisableGameStartRequirement { get; set; } = null!;
    
    [EnumOption(TeamPreferences.Both)]
    [OptionName("Forced team assignment preference")]
    public EnumLocalOption ForcedTeamAssignment { get; set; } = null!;

    private LocalOptions() : base("local")
    {
        TeamPreference.ValueChanged += OnTeamPreferenceValueChanged;
        ForcedTeamAssignment.ValueChanged += OnForcedTeamAssignmentValueChanged;
        
        AllowModdedCosmetics.SetIsHiddenFunc(IsAllowModdedCosmeticsHidden);
        DisableAmDeadCheck.SetIsHiddenFunc(IsDisableAmDeadCheckHidden);
        DisableAmImpostorCheck.SetIsHiddenFunc(IsDisableAmImpostorCheckHidden);
        DisableEndGameChecks.SetIsHiddenFunc(IsDisableEndGameChecksHidden);
        DisableGameStartRequirement.SetIsHiddenFunc(IsDisableGameStartRequirementHidden);
        ForcedTeamAssignment.SetIsHiddenFunc(IsForcedTeamAssignmentHidden);
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
    
    private static bool IsAllowModdedCosmeticsHidden() => BetterVanillaManager.Instance.Features.IsLocked(
        "A617504DA5A04943DE0CF0C85FF2DA7B7C387C6B6D2950E1DF8CB8D0E4D69FCE");
    
    private static bool IsDisableAmDeadCheckHidden() => BetterVanillaManager.Instance.Features.IsLocked(
        "A21B151BDA9AD098622868D3B7EBB5B0BB819EBD7C25B6504BDAAE9FF1FF8C0F");
    
    private static bool IsDisableAmImpostorCheckHidden() => BetterVanillaManager.Instance.Features.IsLocked(
        "B640EC50173A2DAC2056F2926027D3BBDDEFB9A1468627A36164D4101519DDAA");
    
    private static bool IsDisableEndGameChecksHidden() => BetterVanillaManager.Instance.Features.IsLocked(
        "81133C46C6DAACD12259989C7D8D385BE47AF65385F9C3D5BFAFEDD2A5C44FD0");
    
    private static bool IsDisableGameStartRequirementHidden() => BetterVanillaManager.Instance.Features.IsLocked(
        "98F47935FFDEEDC0F716F87A5C332AA77222DF4EAF4FF61E1989EE95B1671B77");
    
    private static bool IsForcedTeamAssignmentHidden() => BetterVanillaManager.Instance.Features.IsLocked(
        "FE3C8AB240748CE91A143E7E61CA6C3D0996D55C23F8F3A7BC42A47BB26B7514");

}