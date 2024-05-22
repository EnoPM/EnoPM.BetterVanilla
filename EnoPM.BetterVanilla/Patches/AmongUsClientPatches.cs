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
}