using BetterVanilla.Components;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(HudManager))]
internal static class HudManagerPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(HudManager.Start))]
    private static void StartPostfix(HudManager __instance)
    {
        __instance.gameObject.AddComponent<ZoomBehaviourManager>();
    }

    [HarmonyPostfix, HarmonyPatch(nameof(HudManager.OnGameStart))]
    private static void OnGameStartPostfix()
    {
        GameEventManager.TriggerGameReallyStarted();
    }
}