using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using BetterVanilla.Options;
using Hazel;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.BulkShareHostToClientSettingChange)]
public sealed class ShareAllHostOptionsRpc : CustomRpcMessage
{
    private byte[] Data { get; }
    
    public ShareAllHostOptionsRpc(BetterPlayerControl sender, byte[] data) : base(sender)
    {
        Data = data;
    }

    public ShareAllHostOptionsRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        var size = reader.ReadInt32();
        Data = reader.ReadBytes(size);
    }
    
    protected override void WriteMessage(MessageWriter writer)
    {
        writer.Write(Data.Length);
        writer.Write(Data);
    }
    
    protected override void HandleMessage()
    {
        HostOptions.Default.FromBytes(Data);
    }
}