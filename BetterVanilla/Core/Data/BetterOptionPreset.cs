using System.IO;
using BetterVanilla.Options;

namespace BetterVanilla.Core.Data;

public sealed class BetterOptionPreset
{
    private bool AllowDeadVoteDisplay { get; }
    private bool AllowTeamPreference { get; }
    private bool HideDeadPlayerPets { get; }
    private float PolusReactorCountdown { get; }
    private bool ProtectFirstKilledPlayer { get; }
    private float ProtectionDuration { get; }
    private bool DefineCommonTasksAsNonCommon { get; }

    public BetterOptionPreset(BinaryReader reader)
    {
        AllowDeadVoteDisplay = reader.ReadBoolean();
        AllowTeamPreference = reader.ReadBoolean();
        HideDeadPlayerPets = reader.ReadBoolean();
        PolusReactorCountdown = reader.ReadSingle();
        if (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            ProtectFirstKilledPlayer = reader.ReadBoolean();
        }
        if (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            ProtectionDuration = reader.ReadSingle();
        }
        if (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            DefineCommonTasksAsNonCommon = reader.ReadBoolean();
        }
    }
    
    public BetterOptionPreset()
    {
        AllowDeadVoteDisplay = HostOptions.Default.AllowDeadVoteDisplay.Value;
        AllowTeamPreference = HostOptions.Default.AllowTeamPreference.Value;
        HideDeadPlayerPets = HostOptions.Default.HideDeadPlayerPets.Value;
        PolusReactorCountdown = HostOptions.Default.PolusReactorCountdown.Value;
        ProtectFirstKilledPlayer = HostOptions.Default.ProtectFirstKilledPlayer.Value;
        ProtectionDuration = HostOptions.Default.ProtectionDuration.Value;
        DefineCommonTasksAsNonCommon = HostOptions.Default.DefineCommonTasksAsNonCommon.Value;
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(AllowDeadVoteDisplay);
        writer.Write(AllowTeamPreference);
        writer.Write(HideDeadPlayerPets);
        writer.Write(PolusReactorCountdown);
        writer.Write(ProtectFirstKilledPlayer);
        writer.Write(ProtectionDuration);
        writer.Write(DefineCommonTasksAsNonCommon);
    }

    public void Apply()
    {
        HostOptions.Default.AllowDeadVoteDisplay.Value = AllowDeadVoteDisplay;
        HostOptions.Default.AllowTeamPreference.Value = AllowTeamPreference;
        HostOptions.Default.HideDeadPlayerPets.Value = HideDeadPlayerPets;
        HostOptions.Default.PolusReactorCountdown.Value = PolusReactorCountdown;
        HostOptions.Default.ProtectFirstKilledPlayer.Value = ProtectFirstKilledPlayer;
        HostOptions.Default.ProtectionDuration.Value = ProtectionDuration;
        HostOptions.Default.DefineCommonTasksAsNonCommon.Value = DefineCommonTasksAsNonCommon;
    }
}