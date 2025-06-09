using BetterVanilla.Cosmetics.Hats;
using System.Linq;
using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core.Bundle;
using BetterVanilla.Cosmetics.Core.Spritesheet;
using BetterVanilla.Cosmetics.Visors;

namespace BetterVanilla.Cosmetics;

public static class CosmeticsContext
{
    public static HatCosmeticManager Hats { get; }
    public static VisorCosmeticManager Visors { get; }

    static CosmeticsContext()
    {
        Hats = new HatCosmeticManager();
        Visors = new VisorCosmeticManager();
    }

    public static void UpdateAnimationFrames()
    {
        Hats.UpdateAnimationFrames();
        Visors.UpdateAnimationFrames();
    }

    public static void RefreshAnimationFrames(PlayerPhysics playerPhysics)
    {
        Hats.RefreshAnimationFrames(playerPhysics);
        Visors.RefreshAnimationFrames(playerPhysics);
    }

    public static void AddBundle(CosmeticBundle bundle)
    {
        var spritesheetCache = new SpritesheetCache(bundle.AllSpritesheet);

        foreach (var key in bundle.AllSpritesheet.Keys)
        {
            CosmeticsPlugin.Logging.LogInfo($"Registering spritesheet: {key}");
        }
        
        CosmeticsPlugin.Logging.LogInfo($"[Hats] Registering {bundle.Hats.Count} cosmetics");
        RegisterHats(bundle, spritesheetCache);
        
        CosmeticsPlugin.Logging.LogInfo($"[Visors] Registering {bundle.Visors.Count} cosmetics");
        RegisterVisors(bundle, spritesheetCache);
    }

    private static void RegisterHats(CosmeticBundle bundle, SpritesheetCache cache)
    {
        foreach (var serialized in bundle.Hats)
        {
            var cosmetic = new HatCosmetic(serialized, cache);
            Hats.AddCosmetic(cosmetic);
        }
    }
    
    private static void RegisterVisors(CosmeticBundle bundle, SpritesheetCache cache)
    {
        foreach (var serialized in bundle.Visors)
        {
            var cosmetic = new VisorCosmetic(serialized, cache);
            Visors.AddCosmetic(cosmetic);
        }
    }
}