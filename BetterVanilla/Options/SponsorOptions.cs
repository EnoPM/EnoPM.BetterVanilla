using BetterVanilla.Components;
using BetterVanilla.Options.Core.Local;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options;

public sealed class SponsorOptions : AbstractSerializableOptionHolder
{
    public static readonly SponsorOptions Default = new();

    [TextOption("Sponsor")]
    [OptionName("Sponsor text")]
    public TextSerializableOption SponsorText { get; set; } = null!;

    [ColorOption("#0000FF")]
    [OptionName("Sponsor text color")]
    public ColorLocalOption SponsorTextColor { get; set; } = null!;

    [NumberOption(1f, 1f, 100f, IncrementValue = 1f)]
    [OptionName("Change your level")]
    public NumberLocalOption LevelOverride { get; set; } = null!;

    [ColorOption("#95CADC")]
    [OptionName("Change the visor color")]
    public ColorLocalOption VisorColor { get; set; } = null!;

    private SponsorOptions() : base("sponsor")
    {
        foreach (var option in GetOptions())
        {
            option.SetIsLockedFunc(IsOptionLocked);
            option.SetLockedText("Available for sponsors");
        }
    }

    private static bool IsOptionLocked()
    {
        if (BetterVanillaManager.Instance.Features.Registry == null) return true;
        if (!EOSManager.Instance || string.IsNullOrEmpty(EOSManager.Instance.FriendCode)) return true;
        return !BetterVanillaManager.Instance.Features.Registry.ContributorFriendCodes
            .Contains(EOSManager.Instance.FriendCode);
    }
}