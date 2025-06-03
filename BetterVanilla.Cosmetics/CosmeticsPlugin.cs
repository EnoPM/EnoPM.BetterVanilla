using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BetterVanilla.Cosmetics.GeneratedRuntime;
using HarmonyLib;

namespace BetterVanilla.Cosmetics;

[BepInPlugin(GeneratedProps.Guid, GeneratedProps.Name, GeneratedProps.Version)]
public sealed class CosmeticsPlugin : BasePlugin
{
    private static readonly Harmony Harmony = new(GeneratedProps.Guid);
    internal static ManualLogSource Logging { get; private set; }
    
    public override void Load()
    {
        Logging = Log;
        Harmony.PatchAll();
    }
}