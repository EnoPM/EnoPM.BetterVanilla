using System.Linq;
using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using Hazel;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.ShareRandomizedMeetingOrder)]
public sealed class RandomizedMeetingOrderRpc : CustomRpcMessage
{
    private byte[] Ids { get; }
    
    public RandomizedMeetingOrderRpc(BetterPlayerControl sender, byte[] ids) : base(sender)
    {
        Ids = ids;
    }

    public RandomizedMeetingOrderRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        var size = reader.ReadInt32();
        Ids = reader.ReadBytes(size);
    }
    
    protected override void WriteMessage(MessageWriter writer)
    {
        writer.Write(Ids.Length);
        writer.Write(Ids);
    }
    
    protected override void HandleMessage()
    {
        if (MeetingHud.Instance == null)
        {
            Ls.LogError($"Unable to handle {nameof(RandomizedMeetingOrderRpc)} without meeting");
            return;
        }
        MeetingHud.Instance.RandomizeVoteAreaPositions(Ids.ToList());
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