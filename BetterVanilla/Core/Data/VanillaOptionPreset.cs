using System.IO;
using AmongUs.GameOptions;

namespace BetterVanilla.Core.Data;

public sealed class VanillaOptionPreset
{
    private byte MapId { get; }
    private float PlayerSpeedMod { get; }
    private float CrewLightMod { get; }
    private float ImpostorLightMod { get; }
    private float KillCooldown { get; }
    private int NumCommonTasks { get; }
    private int NumLongTasks { get; }
    private int NumShortTasks { get; }
    private int NumEmergencyMeetings { get; }
    private bool GhostsDoTasks { get; }
    private int KillDistance { get; }
    private int DiscussionTime { get; }
    private int VotingTime { get; }
    private bool ConfirmImpostors { get; }
    private bool VisualTasks { get; }
    private bool AnonymousVotes { get; }
    private int TaskBarMode { get; }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(MapId);
        writer.Write(PlayerSpeedMod);
        writer.Write(CrewLightMod);
        writer.Write(ImpostorLightMod);
        writer.Write(KillCooldown);
        writer.Write(NumCommonTasks);
        writer.Write(NumLongTasks);
        writer.Write(NumShortTasks);
        writer.Write(NumEmergencyMeetings);
        writer.Write(GhostsDoTasks);
        writer.Write(KillDistance);
        writer.Write(DiscussionTime);
        writer.Write(VotingTime);
        writer.Write(ConfirmImpostors);
        writer.Write(VisualTasks);
        writer.Write(AnonymousVotes);
        writer.Write(TaskBarMode);
    }

    public VanillaOptionPreset(BinaryReader reader)
    {
        MapId = reader.ReadByte();
        PlayerSpeedMod = reader.ReadSingle();
        CrewLightMod = reader.ReadSingle();
        ImpostorLightMod = reader.ReadSingle();
        KillCooldown = reader.ReadSingle();
        NumCommonTasks = reader.ReadInt32();
        NumLongTasks = reader.ReadInt32();
        NumShortTasks = reader.ReadInt32();
        NumEmergencyMeetings = reader.ReadInt32();
        GhostsDoTasks = reader.ReadBoolean();
        KillDistance = reader.ReadInt32();
        DiscussionTime = reader.ReadInt32();
        VotingTime = reader.ReadInt32();
        ConfirmImpostors = reader.ReadBoolean();
        VisualTasks = reader.ReadBoolean();
        AnonymousVotes = reader.ReadBoolean();
        TaskBarMode = reader.ReadInt32();
    }

    public VanillaOptionPreset(NormalGameOptionsV09 options)
    {
        MapId = options.MapId;
        PlayerSpeedMod = options.PlayerSpeedMod;
        CrewLightMod = options.CrewLightMod;
        ImpostorLightMod = options.ImpostorLightMod;
        KillCooldown = options.KillCooldown;
        NumCommonTasks = options.NumCommonTasks;
        NumLongTasks = options.NumLongTasks;
        NumShortTasks = options.NumShortTasks;
        NumEmergencyMeetings = options.NumEmergencyMeetings;
        GhostsDoTasks = options.GhostsDoTasks;
        KillDistance = options.KillDistance;
        DiscussionTime = options.DiscussionTime;
        VotingTime = options.VotingTime;
        ConfirmImpostors = options.ConfirmImpostor;
        VisualTasks = options.VisualTasks;
        AnonymousVotes = options.AnonymousVotes;
        TaskBarMode = (int)options.TaskBarMode;
    }

    public void Apply(IGameOptions options)
    {
        options.SetByte(ByteOptionNames.MapId, MapId);
        options.SetFloat(FloatOptionNames.PlayerSpeedMod, PlayerSpeedMod);
        options.SetFloat(FloatOptionNames.CrewLightMod, CrewLightMod);
        options.SetFloat(FloatOptionNames.ImpostorLightMod, ImpostorLightMod);
        options.SetFloat(FloatOptionNames.KillCooldown, KillCooldown);
        options.SetInt(Int32OptionNames.NumCommonTasks, NumCommonTasks);
        options.SetInt(Int32OptionNames.NumLongTasks, NumLongTasks);
        options.SetInt(Int32OptionNames.NumShortTasks, NumShortTasks);
        options.SetInt(Int32OptionNames.NumEmergencyMeetings, NumEmergencyMeetings);
        options.SetBool(BoolOptionNames.GhostsDoTasks, GhostsDoTasks);
        options.SetInt(Int32OptionNames.KillDistance, KillDistance);
        options.SetInt(Int32OptionNames.DiscussionTime, DiscussionTime);
        options.SetInt(Int32OptionNames.VotingTime, VotingTime);
        options.SetBool(BoolOptionNames.ConfirmImpostor, ConfirmImpostors);
        options.SetBool(BoolOptionNames.VisualTasks, VisualTasks);
        options.SetBool(BoolOptionNames.AnonymousVotes, AnonymousVotes);
        options.SetInt(Int32OptionNames.TaskBarMode, TaskBarMode);
    }
}