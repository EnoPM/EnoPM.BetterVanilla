using BetterVanilla.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(MapBehaviour))]
internal static class MapBehaviourPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(MapBehaviour.FixedUpdate))]
    private static void FixedUpdatePostfix(MapBehaviour __instance)
    {
        __instance.BetterFixedUpdate();
    }

    [HarmonyPrefix, HarmonyPatch(nameof(MapBehaviour.Show))]
    private static void ShowPrefix(MapBehaviour __instance, MapOptions opts)
    {
        __instance.BetterShow(opts);
    }
}