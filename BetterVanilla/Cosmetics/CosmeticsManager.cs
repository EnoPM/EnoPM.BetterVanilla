using BetterVanilla.Components;
using BetterVanilla.Core;
using BetterVanilla.Cosmetics.Api.Core.Bundle;
using BetterVanilla.Cosmetics.Core.Spritesheet;
using BetterVanilla.Cosmetics.Hats;
using BetterVanilla.Cosmetics.Visors;

namespace BetterVanilla.Cosmetics;

public static class CosmeticsManager
{
    public static HatCosmeticManager Hats { get; }
    public static VisorCosmeticManager Visors { get; }

    static CosmeticsManager()
    {
        Hats = new HatCosmeticManager();
        Visors = new VisorCosmeticManager();
    }
    
    public static void RegisterBundle(CosmeticBundle bundle)
    {
        var cache = new SpritesheetCache(bundle.AllSpritesheet);
        
        Ls.LogInfo($"[Hats] Registering {bundle.Hats.Count} cosmetics");
        foreach (var serialized in bundle.Hats)
        {
            var cosmetic = new HatCosmetic(serialized, cache);
            Hats.AddCosmetic(cosmetic);
        }
        
        Ls.LogInfo($"[Visors] Registering {bundle.Visors.Count} cosmetics");
        foreach (var serialized in bundle.Visors)
        {
            var cosmetic = new VisorCosmetic(serialized, cache);
            Visors.AddCosmetic(cosmetic);
        }
    }
    
    #region Helpers

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

    public static void ProcessUnregisteredCosmetics()
    {
        Hats.RegisterCosmetics();
        Visors.RegisterCosmetics();
    }

    public static bool IsUnlocked(string productId)
    {
        if (FeatureCodeBehaviour.Instance == null) return false;
        if (FeatureCodeBehaviour.Instance.SponsorCosmetics.Contains(productId))
        {
            return LocalConditions.AmSponsor();
        }
        if (FeatureCodeBehaviour.Instance.IsCosmeticUnlockable(productId))
        {
            return FeatureCodeBehaviour.Instance.IsCosmeticUnlocked(productId);
        }
        return true;
    }

    #endregion
}