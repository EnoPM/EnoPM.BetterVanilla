using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using Hazel;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.BetterVanillaHandshake)]
public sealed class HandshakeRpc : CustomRpcMessage
{
    private BetterVanillaHandshake Handshake { get; }
    
    public HandshakeRpc(BetterPlayerControl sender, BetterVanillaHandshake handshake) : base(sender)
    {
        Handshake = handshake;
    }

    public HandshakeRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        Handshake = new BetterVanillaHandshake(reader);
    }
    
    protected override void WriteMessage(MessageWriter writer)
    {
        Handshake.Serialize(writer);
    }
    
    protected override void HandleMessage()
    {
        Sender.SetHandshake(Handshake);
    }
}