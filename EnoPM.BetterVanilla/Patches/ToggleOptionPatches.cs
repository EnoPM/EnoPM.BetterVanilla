using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(ToggleOption))]
internal static class ToggleOptionPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(ToggleOption.UpdateValue))]
    private static void UpdateValuePostfix(ToggleOption __instance)
    {
        ModSettings.Host.OnUpdatedByVanillaUi(__instance);
    }
}