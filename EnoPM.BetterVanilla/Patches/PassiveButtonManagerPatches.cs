using EnoPM.BetterVanilla.Core;
using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(PassiveButtonManager))]
internal static class PassiveButtonManagerPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(PassiveButtonManager.Update))]
    private static bool UpdatePrefix()
    {
        return !PassiveButtonsBlocker.ShouldBlock();
    }
}