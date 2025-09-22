using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using Hazel;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.TeamPreference)]
public sealed class TeamPreferenceRpc : CustomRpcMessage
{
    private TeamPreferences TeamPreference { get; }
    
    public TeamPreferenceRpc(BetterPlayerControl sender, TeamPreferences preference) : base(sender)
    {
        TeamPreference = preference;
    }

    public TeamPreferenceRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        TeamPreference = (TeamPreferences)reader.ReadUInt32();
    }
    
    protected override void WriteMessage(MessageWriter writer)
    {
        writer.Write((uint)TeamPreference);
    }
    
    protected override void HandleMessage()
    {
        BetterVanillaManager.Instance.AllTeamPreferences[Sender.Player.OwnerId] = TeamPreference;
    }

    public override void Send(BetterPlayerControl? receiver = null)
    {
        if (AmongUsClient.Instance.AmClient)
        {
            HandleMessage();
        }
        base.Send(receiver);
    }
}