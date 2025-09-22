using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using Hazel;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.PrivateChatMessage)]
public sealed class PrivateChatMessageRpc : CustomRpcMessage
{
    private int ReceiverOwnerId { get; }
    private string Message { get; }
    
    public PrivateChatMessageRpc(BetterPlayerControl sender, int receiverOwnerId, string message) : base(sender)
    {
        ReceiverOwnerId = receiverOwnerId;
        Message = message;
    }

    public PrivateChatMessageRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        ReceiverOwnerId = reader.ReadInt32();
        Message = reader.ReadString();
    }
    
    protected override void WriteMessage(MessageWriter writer)
    {
        writer.Write(ReceiverOwnerId);
        writer.Write(Message);
    }
    
    protected override void HandleMessage()
    {
        if (HudManager.Instance == null || HudManager.Instance.Chat == null) return;
        ChatCommandsManager.HandlePrivateMessageRpc(Sender.Player, ReceiverOwnerId, Message);
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