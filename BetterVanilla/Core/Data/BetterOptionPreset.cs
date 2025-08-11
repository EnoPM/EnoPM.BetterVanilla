using System.IO;
using BetterVanilla.Options;

namespace BetterVanilla.Core.Data;

public sealed class BetterOptionPreset
{
    private bool AllowDeadVoteDisplay { get; }
    private bool AllowTeamPreference { get; }
    private bool HideDeadPlayerPets { get; }
    private float PolusReactorCountdown { get; }

    public BetterOptionPreset(BinaryReader reader)
    {
        AllowDeadVoteDisplay = reader.ReadBoolean();
        AllowTeamPreference = reader.ReadBoolean();
        HideDeadPlayerPets = reader.ReadBoolean();
        PolusReactorCountdown = reader.ReadSingle();
    }
    
    public BetterOptionPreset()
    {
        AllowDeadVoteDisplay = HostOptions.Default.AllowDeadVoteDisplay.Value;
        AllowTeamPreference = HostOptions.Default.AllowTeamPreference.Value;
        HideDeadPlayerPets = HostOptions.Default.HideDeadPlayerPets.Value;
        PolusReactorCountdown = HostOptions.Default.PolusReactorCountdown.Value;
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(AllowDeadVoteDisplay);
        writer.Write(AllowTeamPreference);
        writer.Write(HideDeadPlayerPets);
        writer.Write(PolusReactorCountdown);
    }

    public void Apply()
    {
        HostOptions.Default.AllowDeadVoteDisplay.Value = AllowDeadVoteDisplay;
        HostOptions.Default.AllowTeamPreference.Value = AllowTeamPreference;
        HostOptions.Default.HideDeadPlayerPets.Value = HideDeadPlayerPets;
        HostOptions.Default.PolusReactorCountdown.Value = PolusReactorCountdown;
    }
}