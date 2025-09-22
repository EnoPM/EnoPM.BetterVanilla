using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using Hazel;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.ForcedTeamAssignment)]
public sealed class ForcedTeamAssignmentRpc : CustomRpcMessage
{
    private TeamPreferences TeamAssignment { get; }
    
    public ForcedTeamAssignmentRpc(BetterPlayerControl sender, TeamPreferences assignment) : base(sender)
    {
        TeamAssignment = assignment;
    }

    public ForcedTeamAssignmentRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        TeamAssignment = (TeamPreferences)reader.ReadUInt32();
    }
    
    protected override void WriteMessage(MessageWriter writer)
    {
        writer.Write((uint)TeamAssignment);
    }
    
    protected override void HandleMessage()
    {
        BetterVanillaManager.Instance.AllForcedTeamAssignments[Sender.Player.OwnerId] = TeamAssignment;
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