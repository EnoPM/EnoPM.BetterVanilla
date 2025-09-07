using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using Hazel;
using UnityEngine;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.ShareSponsorVisorColor)]
public sealed class SponsorVisorColorRpc : CustomRpcMessage
{
    private Color SponsorVisorColor { get; }

    public SponsorVisorColorRpc(BetterPlayerControl sender, Color visorColor) : base(sender)
    {
        SponsorVisorColor = visorColor;
    }

    public SponsorVisorColorRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        SponsorVisorColor = reader.ReadColor();
    }

    protected override void WriteMessage(MessageWriter writer)
    {
        writer.Write(SponsorVisorColor);
    }

    protected override void HandleMessage()
    {
        Sender.SetVisorColor(SponsorVisorColor);
    }
}