using BepInEx;
using BepInEx.Unity.IL2CPP;
using BetterVanilla.Compiler;
using BetterVanilla.Components;
using BetterVanilla.Core;

namespace BetterVanilla;

[BepInPlugin(GeneratedProps.Guid, GeneratedProps.Name, GeneratedProps.Version)]
public class BetterVanillaPlugin : BasePlugin
{
    public override void Load()
    {
        Ls.SetLogSource(Log);
        AddComponent<BetterVanillaManager>();
    }
}