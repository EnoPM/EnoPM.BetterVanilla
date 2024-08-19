using EnoPM.BetterVanilla.Components;
using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(AmongUsClient))]
internal static class AmongUsClientPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(AmongUsClient.OnGameEnd))]
    private static void OnGameEndPrefix()
    {
        if (ZoomBehaviour.Instance)
        {
            ZoomBehaviour.Instance.ResetZoom();
        }
    }

    [HarmonyPostfix, HarmonyPatch(nameof(AmongUsClient.OnBecomeHost))]
    private static void OnBecomeHostPostfix(AmongUsClient __instance)
    {
        // TODO: Setup vanilla settings from BetterVanilla storage
    }
}