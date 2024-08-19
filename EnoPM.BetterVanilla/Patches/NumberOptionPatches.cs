using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(NumberOption))]
internal static class NumberOptionPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(NumberOption.UpdateValue))]
    private static void UpdateValuePostfix(NumberOption __instance)
    {
        ModSettings.Host.OnUpdatedByVanillaUi(__instance);
    }
}