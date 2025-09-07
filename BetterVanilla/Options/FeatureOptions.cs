using BetterVanilla.Components;
using BetterVanilla.Core.Data;
using BetterVanilla.Options.Core.Local;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options;

public sealed class FeatureOptions : AbstractSerializableOptionHolder
{
    public static readonly FeatureOptions Default = new();
    
    [BoolOption(false)]
    [OptionName("Display Modded Cosmetics")]
    [HiddenUnderHash("A617504DA5A04943DE0CF0C85FF2DA7B7C387C6B6D2950E1DF8CB8D0E4D69FCE")]
    public BoolLocalOption AllowModdedCosmetics { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Disable Am Dead Check")]
    [HiddenUnderHash("A21B151BDA9AD098622868D3B7EBB5B0BB819EBD7C25B6504BDAAE9FF1FF8C0F")]
    public BoolLocalOption DisableAmDeadCheck { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Disable Am Impostor Check")]
    [HiddenUnderHash("B640EC50173A2DAC2056F2926027D3BBDDEFB9A1468627A36164D4101519DDAA")]
    public BoolLocalOption DisableAmImpostorCheck { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Disable All End Game Checks")]
    [HiddenUnderHash("81133C46C6DAACD12259989C7D8D385BE47AF65385F9C3D5BFAFEDD2A5C44FD0")]
    public BoolLocalOption DisableEndGameChecks { get; set; } = null!;
    
    [BoolOption(false)]
    [OptionName("Disable Start Game Player Requirement")]
    [HiddenUnderHash("98F47935FFDEEDC0F716F87A5C332AA77222DF4EAF4FF61E1989EE95B1671B77")]
    public BoolLocalOption DisableGameStartRequirement { get; set; } = null!;
    
    [EnumOption(TeamPreferences.Both)]
    [OptionName("Forced team assignment preference")]
    [HiddenUnderHash("FE3C8AB240748CE91A143E7E61CA6C3D0996D55C23F8F3A7BC42A47BB26B7514")]
    public EnumLocalOption ForcedTeamAssignment { get; set; } = null!;


    private FeatureOptions() : base("feature")
    {
        ForcedTeamAssignment.ValueChanged += OnForcedTeamAssignmentValueChanged;
    }
    
    private void OnForcedTeamAssignmentValueChanged()
    {
        if (BetterPlayerControl.LocalPlayer == null || AmongUsClient.Instance == null) return;

        BetterPlayerControl.LocalPlayer.RpcSetForcedTeamAssignment(ForcedTeamAssignment.ParseValue(TeamPreferences.Both));
    }
}