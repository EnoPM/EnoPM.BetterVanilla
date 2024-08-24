using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(ProgressionScreen))]
internal static class ProgressionScreenPatches
{
    [HarmonyPrefix, HarmonyPatch(typeof(ProgressionScreen._DoAnimations_d__14), nameof(ProgressionScreen._DoAnimations_d__14.MoveNext))]
    private static bool _DoAnimations_d__14MoveNextPrefix(ProgressionScreen._DoAnimations_d__14 __instance)
    {
        var progressionScreen = __instance.__4__this;
        progressionScreen.StartCoroutine(progressionScreen.CoDoAnimations(__instance.xpGainedResult));
        return false;
    }
}