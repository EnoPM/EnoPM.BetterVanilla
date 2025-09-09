using BetterVanilla.Core.Extensions;
using BetterVanilla.Options;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(PlayerTab))]
internal static class PlayerTabPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerTab.SelectColor))]
    private static void SelectColorPostfix(PlayerTab __instance)
    {
        __instance.PlayerPreview.SetLocalVisorColor();
    }
}