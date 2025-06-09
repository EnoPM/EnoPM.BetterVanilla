using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BetterVanilla.Cosmetics.Api.Core.Bundle;
using BetterVanilla.Cosmetics.GeneratedRuntime;
using HarmonyLib;

namespace BetterVanilla.Cosmetics;

[BepInPlugin(GeneratedProps.Guid, GeneratedProps.Name, GeneratedProps.Version)]
public sealed class CosmeticsPlugin : BasePlugin
{
    private static readonly Harmony Harmony = new(GeneratedProps.Guid);
    internal static ManualLogSource Logging => Instance.Log;
    private static CosmeticsPlugin Instance { get; set; } = null!;

    public override void Load()
    {
        Instance = this;
        Harmony.PatchAll();

        var bundle = CosmeticBundle.FromFile(@"D:\GameDevelopment\AmongUs\Spritesheets\BetterVanillaCosmetics.bundle");
        CosmeticsContext.AddBundle(bundle);
    }
}