using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(PlanetSurveillanceMinigame))]
internal static class PlanetSurveillanceMinigamePatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlanetSurveillanceMinigame.Begin))]
    private static void BeginPostfix(PlanetSurveillanceMinigame __instance)
    {
        if (!LocalConditions.ShouldAnonymizePlayers()) return;
        Ls.LogMessage($"Cameras begin");
        __instance.StartCoroutine(PlayerAnonymizer.CoEnable(0.25f));
    }

    [HarmonyPrefix, HarmonyPatch(nameof(PlanetSurveillanceMinigame.OnDestroy))]
    private static void OnDestroyPrefix(PlanetSurveillanceMinigame __instance)
    {
        Ls.LogMessage($"Cameras end");
        PlayerAnonymizer.Disable();
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PlanetSurveillanceMinigame.Update))]
    private static void UpdatePostfix(PlanetSurveillanceMinigame __instance)
    {
        if (!PlayerAnonymizer.IsActive || LocalConditions.ShouldAnonymizePlayers())
        {
            PlayerAnonymizer.Disable();
        }
        else if (!PlayerAnonymizer.IsActive && LocalConditions.ShouldAnonymizePlayers())
        {
            PlayerAnonymizer.Enable();
        }
    }
}