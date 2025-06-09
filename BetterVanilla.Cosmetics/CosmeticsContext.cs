using BetterVanilla.Cosmetics.Api.Bundle;
using BetterVanilla.Cosmetics.Hats;
using System.Linq;
using System.Text.Json;
using BetterVanilla.Cosmetics.Core.Spritesheet;

namespace BetterVanilla.Cosmetics;

public static class CosmeticsContext
{
    public static HatCosmeticManager Hats { get; }

    static CosmeticsContext()
    {
        Hats = new HatCosmeticManager();
    }

    public static void UpdateAnimationFrames()
    {
        Hats.UpdateAnimationFrames();
    }

    public static void AddBundle(CosmeticBundle bundle)
    {
        var spritesheetCache = new SpritesheetCache(bundle.AllSpritesheet);
        
        CosmeticsPlugin.Logging.LogMessage($"Adding bundle to {JsonSerializer.Serialize(bundle.AllSpritesheet.Keys.ToList())}");
        CosmeticsPlugin.Logging.LogMessage($"Adding {bundle.Hats.Count} hats");

        RegisterHats(bundle, spritesheetCache);
    }

    private static void RegisterHats(CosmeticBundle bundle, SpritesheetCache cache)
    {
        foreach (var serialized in bundle.Hats)
        {
            var cosmetic = new HatCosmetic(serialized, cache);
            Hats.AddCosmetic(cosmetic);
        }
    }
}