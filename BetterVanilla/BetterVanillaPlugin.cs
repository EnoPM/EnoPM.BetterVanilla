using BepInEx;
using BepInEx.Unity.IL2CPP;
using BetterVanilla.Compiler;
using BetterVanilla.Components;
using BetterVanilla.Core;
using HarmonyLib;

namespace BetterVanilla;

[BepInPlugin(GeneratedProps.Guid, GeneratedProps.Name, GeneratedProps.Version)]
public class BetterVanillaPlugin : BasePlugin
{
    public override void Load()
    {
        Ls.SetLogSource(Log);
        AddComponent<BetterVanillaManager>();
        Ls.LogInfo($"Plugin {GeneratedProps.Name} v{GeneratedProps.Version} is loaded!");
        var harmony = new Harmony(GeneratedProps.Guid);
        harmony.PatchAll();
    }
}