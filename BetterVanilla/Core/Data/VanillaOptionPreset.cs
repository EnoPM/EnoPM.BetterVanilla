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
    private int EmergencyCooldown { get; }
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
        writer.Write(EmergencyCooldown);
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
        EmergencyCooldown = reader.ReadInt32();
        GhostsDoTasks = reader.ReadBoolean();
        KillDistance = reader.ReadInt32();
        DiscussionTime = reader.ReadInt32();
        VotingTime = reader.ReadInt32();
        ConfirmImpostors = reader.ReadBoolean();
        VisualTasks = reader.ReadBoolean();
        AnonymousVotes = reader.ReadBoolean();
        TaskBarMode = reader.ReadInt32();
    }

    public VanillaOptionPreset(IGameOptions options)
    {
        MapId = options.MapId;
        PlayerSpeedMod = options.TryGetFloat(FloatOptionNames.PlayerSpeedMod, out var playerSpeedMod) ? playerSpeedMod : 0f;
        CrewLightMod = options.TryGetFloat(FloatOptionNames.CrewLightMod, out var crewLightMod) ? crewLightMod : 0f;
        ImpostorLightMod = options.TryGetFloat(FloatOptionNames.ImpostorLightMod, out var impostorLightMod) ? impostorLightMod : 0f;
        KillCooldown = options.TryGetFloat(FloatOptionNames.KillCooldown, out var killCooldown) ? killCooldown : 0f;
        NumCommonTasks = options.TryGetInt(Int32OptionNames.NumCommonTasks, out var numCommonTasks) ? numCommonTasks : 0;
        NumLongTasks = options.TryGetInt(Int32OptionNames.NumLongTasks, out var numLongTasks) ? numLongTasks : 0;
        NumShortTasks = options.TryGetInt(Int32OptionNames.NumShortTasks, out var numShortTasks) ? numShortTasks : 0;
        NumEmergencyMeetings = options.TryGetInt(Int32OptionNames.NumEmergencyMeetings, out var numEmergencyMeetings) ? numEmergencyMeetings : 0;
        EmergencyCooldown = options.TryGetInt(Int32OptionNames.EmergencyCooldown, out var emergencyCooldown) ? emergencyCooldown : 0;
        GhostsDoTasks = options.TryGetBool(BoolOptionNames.GhostsDoTasks, out var ghostsDoTasks) && ghostsDoTasks;
        KillDistance = options.TryGetInt(Int32OptionNames.KillDistance, out var killDistance) ? killDistance : 0;
        DiscussionTime = options.TryGetInt(Int32OptionNames.DiscussionTime, out var discussionTime) ? discussionTime : 0;
        VotingTime = options.TryGetInt(Int32OptionNames.VotingTime, out var votingTime) ? votingTime : 0;
        ConfirmImpostors = options.TryGetBool(BoolOptionNames.ConfirmImpostor, out var confirmImpostors) && confirmImpostors;
        VisualTasks = options.TryGetBool(BoolOptionNames.VisualTasks, out var visualTasks) && visualTasks;
        AnonymousVotes = options.TryGetBool(BoolOptionNames.AnonymousVotes, out var anonymousVotes) && anonymousVotes;
        TaskBarMode = options.TryGetInt(Int32OptionNames.TaskBarMode, out var taskBarMode) ? taskBarMode : 0;
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
        options.SetInt(Int32OptionNames.EmergencyCooldown, EmergencyCooldown);
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