namespace BetterVanilla.Cosmetics.Hats.Extensions;

internal static class HatDataExtensions
{
    internal static bool IsCached(this HatData hat)
    {
        return CosmeticsContext.Hats.TryGetViewData(hat.name, out _);
    }
}