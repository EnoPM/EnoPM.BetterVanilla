using System;
using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options.Core.Local;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options;

public sealed class SponsorOptions : AbstractSerializableOptionHolder
{
    public static readonly SponsorOptions Default = new();

    [TextOption("Sponsor")]
    [OptionName("Sponsor text")]
    public TextLocalOption SponsorText { get; set; } = null!;
    private Debouncer SponsorTextDebouncer { get; }

    [ColorOption("#0000FF")]
    [OptionName("Sponsor text color")]
    public ColorLocalOption SponsorTextColor { get; set; } = null!;
    private Debouncer SponsorTextColorDebouncer { get; }

    [NumberOption(1f, 1f, 100f, IncrementValue = 1f)]
    [OptionName("Change your level")]
    public NumberLocalOption LevelOverride { get; set; } = null!;

    [ColorOption("#95CADC")]
    [OptionName("Change the visor color")]
    public ColorLocalOption VisorColor { get; set; } = null!;
    private Debouncer VisorColorDebouncer { get; }

    private SponsorOptions() : base("sponsor")
    {
        foreach (var option in GetOptions())
        {
            option.SetIsLockedFunc(IsOptionLocked);
            option.SetLockedText("Available for sponsors only");
        }

        SponsorTextDebouncer = new Debouncer(TimeSpan.FromSeconds(2));
        SponsorText.ValueChanged += SponsorTextDebouncer.Trigger;
        SponsorTextDebouncer.Debounced += ShareSponsorText;
        
        SponsorTextColorDebouncer = new Debouncer(TimeSpan.FromSeconds(2));
        SponsorTextColor.ValueChanged += SponsorTextColorDebouncer.Trigger;
        SponsorTextColorDebouncer.Debounced += ShareSponsorTextColor;
        
        VisorColorDebouncer = new Debouncer(TimeSpan.FromSeconds(2));
        VisorColor.ValueChanged += VisorColorDebouncer.Trigger;
        VisorColorDebouncer.Debounced += ShareVisorColor;
    }

    private static bool IsOptionLocked()
    {
        if (BetterPlayerControl.LocalPlayer == null || BetterPlayerControl.LocalPlayer.FriendCode == null) return true;
        return !BetterPlayerControl.LocalPlayer.AmSponsor;
    }

    private void ShareSponsorText()
    {
        Ls.LogMessage($"Sharing sponsor text");
        if (PlayerControl.LocalPlayer == null) return;
        PlayerControl.LocalPlayer.RpcShareSponsorText(SponsorText.Value);
    }

    private void ShareSponsorTextColor()
    {
        Ls.LogMessage($"Sharing sponsor text color");
        if (PlayerControl.LocalPlayer == null) return;
        PlayerControl.LocalPlayer.RpcShareSponsorTextColor(SponsorTextColor.Value);
    }

    private void ShareVisorColor()
    {
        Ls.LogMessage($"Sharing visor color");
        if (PlayerControl.LocalPlayer == null) return;
        PlayerControl.LocalPlayer.RpcShareSponsorVisorColor(VisorColor.Value);
    }
}