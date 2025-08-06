using BepInEx;
using BepInEx.Unity.IL2CPP;
using BetterVanilla.GeneratedRuntime;
using BetterVanilla.Components;
using BetterVanilla.Core;

namespace BetterVanilla;

[BepInPlugin(GeneratedProps.Guid, GeneratedProps.Name, GeneratedProps.Version)]
internal sealed class BetterVanillaPlugin : BasePlugin
{
    internal static BetterVanillaPlugin Instance { get; private set; } = null!;

    public override void Load()
    {
        Instance = this;
        Ls.SetLogSource(Log);
        AddComponent<BetterVanillaManager>();
        AddComponent<ModUpdaterBehaviour>();
    }
}