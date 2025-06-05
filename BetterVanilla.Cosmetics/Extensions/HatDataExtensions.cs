using System.Collections.Generic;
using BetterVanilla.Cosmetics.Data;

namespace BetterVanilla.Cosmetics.Extensions;

internal static class HatDataExtensions
{
    public static HatExtension? GetHatExtension(this HatData hat)
    {
        if (CosmeticsManager.TestHatExtension != null && CosmeticsManager.TestHatExtension.Condition.Equals(hat.name))
        {
            return CosmeticsManager.TestHatExtension;
        }

        return CosmeticsManager.HatExtensionCache.GetValueOrDefault(hat.name);
    }
    
    public static bool IsCached(this HatData hat)
    {
        return CosmeticsManager.HatViewDataCache.ContainsKey(hat.name);
    }
}