using BetterVanilla.Components;
using BetterVanilla.Core.Attributes;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using Hazel;
using UnityEngine;

namespace BetterVanilla.Core.Rpc;

[RpcMessage(RpcIds.ShareSponsorTextColor)]
public sealed class SponsorTextColorRpc : CustomRpcMessage
{
    private Color SponsorTextColor { get; }

    public SponsorTextColorRpc(BetterPlayerControl sender, Color textColor) : base(sender)
    {
        SponsorTextColor = textColor;
    }

    public SponsorTextColorRpc(BetterPlayerControl sender, MessageReader reader) : base(sender)
    {
        SponsorTextColor = reader.ReadColor();
    }

    protected override void WriteMessage(MessageWriter writer)
    {
        writer.Write(SponsorTextColor);
    }

    protected override void HandleMessage()
    {
        Sender.SetSponsorTextColor(SponsorTextColor);
    }
}