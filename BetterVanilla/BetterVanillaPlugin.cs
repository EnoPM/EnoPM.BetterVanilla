using BepInEx;
using BepInEx.Unity.IL2CPP;
using BetterVanilla.GeneratedRuntime;
using BetterVanilla.Components;
using BetterVanilla.Core;

namespace BetterVanilla;

[BepInPlugin(GeneratedProps.Guid, GeneratedProps.Name, GeneratedProps.Version)]
internal sealed class BetterVanillaPlugin : BasePlugin
{
    public override void Load()
    {
        Ls.SetLogSource(Log);
        AddComponent<BetterVanillaManager>();
    }
}