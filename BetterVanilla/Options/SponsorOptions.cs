using System;
using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options.Core.Local;
using BetterVanilla.Options.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Options;

public sealed class SponsorOptions : AbstractSerializableOptionHolder
{
    public static readonly SponsorOptions Default = new();

    [TextOption("Sponsor", 15)]
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
    private Debouncer LevelOverrideDebouncer { get; }

    [ColorOption("#95CADC")]
    [OptionName("Change the visor color")]
    public ColorLocalOption VisorColor { get; set; } = null!;
    private Debouncer VisorColorDebouncer { get; }

    private SponsorOptions() : base("sponsor")
    {
        foreach (var option in GetOptions())
        {
            option.SetIsLockedFunc(IsOptionLocked);
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
        
        LevelOverrideDebouncer = new Debouncer(TimeSpan.FromSeconds(2));
        LevelOverride.ValueChanged += LevelOverrideDebouncer.Trigger;
        LevelOverrideDebouncer.Debounced += ShareLevel;
    }

    private bool IsOptionLocked()
    {
        
        if (BetterPlayerControl.LocalPlayer == null)
        {
            foreach (var option in GetOptions())
            {
                option.SetLockedText("Available in lobby");
            }
            return true;
        }
        if (!LocalConditions.AmSponsor())
        {
            foreach (var option in GetOptions())
            {
                option.SetLockedText("Available for sponsors");
            }
            return true;
        }
        if (LocalConditions.IsGameStarted())
        {
            foreach (var option in GetOptions())
            {
                option.SetLockedText("Not available when the game is started");
            }
            return true;
        }
        return !BetterPlayerControl.LocalPlayer.AmSponsor;
    }

    public void ShareSponsorText()
    {
        if (BetterPlayerControl.LocalPlayer == null) return;
        BetterPlayerControl.LocalPlayer.RpcSetSponsorText(SponsorText.Value);
    }

    public void ShareSponsorTextColor()
    {
        if (BetterPlayerControl.LocalPlayer == null) return;
        BetterPlayerControl.LocalPlayer.RpcSetSponsorTextColor(SponsorTextColor.Value);
    }

    public void ShareVisorColor()
    {
        if (BetterPlayerControl.LocalPlayer == null) return;
        BetterPlayerControl.LocalPlayer.RpcSetVisorColor(VisorColor.Value);
    }

    private void ShareLevel()
    {
        if (PlayerControl.LocalPlayer == null || !LocalConditions.AmSponsor()) return;
        PlayerControl.LocalPlayer.RpcSetLevel((uint)Mathf.RoundToInt(LevelOverride.Value));
    }
}