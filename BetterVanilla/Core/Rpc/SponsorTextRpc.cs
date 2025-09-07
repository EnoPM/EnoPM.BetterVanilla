using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using Hazel;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.ShareSponsorText)]
public sealed class SponsorTextRpc : CustomRpcMessage
{
    private string SponsorText { get; }

    public SponsorTextRpc(BetterPlayerControl sender, string sponsorText) : base(sender)
    {
        SponsorText = sponsorText;
    }

    public SponsorTextRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        SponsorText = reader.ReadString();
    }

    protected override void WriteMessage(MessageWriter writer)
    {
        writer.Write(SponsorText);
    }

    protected override void HandleMessage()
    {
        Sender.SetSponsorText(SponsorText);
    }
}