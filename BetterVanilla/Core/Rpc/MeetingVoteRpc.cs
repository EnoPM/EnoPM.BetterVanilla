using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using Hazel;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.SetMeetingVote)]
public sealed class MeetingVoteRpc : CustomRpcMessage
{
    private byte VoterId { get; }
    private byte VotedId { get; }
    
    public MeetingVoteRpc(BetterPlayerControl sender, byte voterId, byte votedId) : base(sender)
    {
        VoterId = voterId;
        VotedId = votedId;
    }

    public MeetingVoteRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        VoterId = reader.ReadByte();
        VotedId = reader.ReadByte();
    }
    
    protected override void WriteMessage(MessageWriter writer)
    {
        writer.Write(VoterId);
        writer.Write(VotedId);
    }
    
    protected override void HandleMessage()
    {
        MeetingHudExtensions.CastVote(VoterId, VotedId);
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