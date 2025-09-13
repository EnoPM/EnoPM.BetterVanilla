using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using Hazel;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.ShareFirstKilledPlayer)]
public sealed class ShareFirstKilledPlayerRpc : CustomRpcMessage
{
    private string FirstKilledPlayer { get; }

    public ShareFirstKilledPlayerRpc(BetterPlayerControl sender, string firstKilledPlayer) : base(sender)
    {
        FirstKilledPlayer = firstKilledPlayer;
    }

    public ShareFirstKilledPlayerRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        FirstKilledPlayer = reader.ReadString();
    }

    protected override void WriteMessage(MessageWriter writer)
    {
        writer.Write(FirstKilledPlayer);
    }

    protected override void HandleMessage()
    {
        PlayerShieldBehaviour.Instance.SetFirstKilledPlayer(FirstKilledPlayer);
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