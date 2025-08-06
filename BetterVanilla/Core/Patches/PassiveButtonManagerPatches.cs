using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(PassiveButtonManager))]
internal static class PassiveButtonManagerPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(PassiveButtonManager.Update))]
    private static bool UpdatePrefix()
    {
        return !UiInteractionBlocker.ShouldBlock();
    }
}