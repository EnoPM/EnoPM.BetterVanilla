using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BetterVanilla.Cosmetics.Api.Core.Bundle;
using BetterVanilla.Cosmetics.Core.Spritesheet;
using BetterVanilla.Cosmetics.GeneratedRuntime;
using BetterVanilla.Cosmetics.Hats;
using BetterVanilla.Cosmetics.Visors;
using HarmonyLib;

namespace BetterVanilla.Cosmetics;

[BepInPlugin(GeneratedProps.Guid, GeneratedProps.Name, GeneratedProps.Version)]
public sealed class CosmeticsPlugin : BasePlugin
{
    internal static ManualLogSource Logging { get; private set; } = null!;
    public static CosmeticsPlugin Instance { get; private set; } = null!;
    private Harmony Harmony { get; }
    
    public HatCosmeticManager Hats { get; }
    public VisorCosmeticManager Visors { get; }

    public CosmeticsPlugin()
    {
        Logging = BepInEx.Logging.Logger.CreateLogSource(nameof(CosmeticsPlugin));
        Instance = this;
        Harmony = new Harmony(GeneratedProps.Guid);
        
        Hats = new HatCosmeticManager();
        Visors = new VisorCosmeticManager();
    }

    public override void Load()
    {
        Instance = this;
        Harmony.PatchAll();

        RegisterBundleFromFile(@"D:\GameDevelopment\AmongUs\Spritesheets\BetterVanillaCosmetics.bundle");
    }

    public void RegisterBundleFromFile(string filePath)
    {
        var bundle = CosmeticBundle.FromFile(filePath);
        RegisterBundle(bundle);
    }

    public void RegisterBundle(CosmeticBundle bundle)
    {
        var cache = new SpritesheetCache(bundle.AllSpritesheet);

        foreach (var key in bundle.AllSpritesheet.Keys)
        {
            Logging.LogInfo($"Registering spritesheet: {key}");
        }
        
        Logging.LogInfo($"[Hats] Registering {bundle.Hats.Count} cosmetics");
        foreach (var serialized in bundle.Hats)
        {
            var cosmetic = new HatCosmetic(serialized, cache);
            Hats.AddCosmetic(cosmetic);
        }
        
        Logging.LogInfo($"[Visors] Registering {bundle.Visors.Count} cosmetics");
        foreach (var serialized in bundle.Visors)
        {
            var cosmetic = new VisorCosmetic(serialized, cache);
            Visors.AddCosmetic(cosmetic);
        }
    }

    #region Helpers

    public void UpdateAnimationFrames()
    {
        Hats.UpdateAnimationFrames();
        Visors.UpdateAnimationFrames();
    }

    public void RefreshAnimationFrames(PlayerPhysics playerPhysics)
    {
        Hats.RefreshAnimationFrames(playerPhysics);
        Visors.RefreshAnimationFrames(playerPhysics);
    }

    #endregion
}