using AmongUs.Data;
using BetterVanilla.Components;
using BetterVanilla.Options.Components;
using BetterVanilla.Options.Core.Local;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options;

public sealed class SponsorOptions() : AbstractSerializableOptionHolder("sponsor")
{
    public static readonly SponsorOptions Default = new();

    [TextOption("Sponsor")]
    [OptionName("Sponsor text")]
    public TextSerializableOption SponsorText { get; set; } = null!;

    [ColorOption("#0000FF")]
    [OptionName("Sponsor text color")]
    public ColorLocalOption SponsorTextColor { get; set; } = null!;

    [NumberOption(1f, 1f, 100f, IncrementValue = 1f)]
    [OptionName("Your level")]
    public NumberLocalOption LevelOverride { get; set; } = null!;
    
    [ColorOption("#95CADC")]
    [OptionName("Visor color")]
    public ColorLocalOption VisorColor { get; set; } = null!;

    public void UpdateLevelOverrideOption()
    {
        var level = DataManager.Player.Stats.Level + BetterVanillaManager.Instance.Database.Data.PlayerLevel;
        LevelOverride.MaxValue = level;
        
        foreach (var option in BetterVanillaManager.Instance.BetterMenu.Ui.sponsorTab.AllOptions)
        {
            if (option is not NumberOptionUi numberOption) continue;
            if (numberOption.SerializableOption != LevelOverride) continue;
            numberOption.RefreshOptionStates();
        }
    }
}