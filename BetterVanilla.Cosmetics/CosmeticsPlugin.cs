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
    internal static ManualLogSource Logging => Instance.Log;
    internal static CosmeticsPlugin Instance { get; private set; } = null!;
    
    public override void Load()
    {
        Instance = this;
        Harmony.PatchAll();
        
        CosmeticsManager.Register("EnoPM/BetterOtherHats", "CustomHats.json");
    }
}