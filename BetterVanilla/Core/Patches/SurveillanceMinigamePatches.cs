using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(SurveillanceMinigame))]
internal static class SurveillanceMinigamePatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(SurveillanceMinigame.Begin))]
    private static void BeginPostfix(SurveillanceMinigame __instance)
    {
        if (!LocalConditions.ShouldAnonymizePlayers()) return;
        Ls.LogMessage($"Cameras begin");
        __instance.StartCoroutine(PlayerAnonymizer.CoEnable(0.25f));
    }

    [HarmonyPostfix, HarmonyPatch(nameof(SurveillanceMinigame.OnDestroy))]
    private static void OnDestroyPostfix(SurveillanceMinigame __instance)
    {
        Ls.LogMessage($"Cameras end");
        PlayerAnonymizer.Disable();
    }
    [HarmonyPostfix, HarmonyPatch(nameof(PlanetSurveillanceMinigame.Update))]
    private static void UpdatePostfix(PlanetSurveillanceMinigame __instance)
    {
        if (PlayerAnonymizer.IsActive && !LocalConditions.ShouldAnonymizePlayers())
        {
            PlayerAnonymizer.Disable();
        }
        else if (!PlayerAnonymizer.IsActive && LocalConditions.ShouldAnonymizePlayers())
        {
            PlayerAnonymizer.Enable();
        }
    }
    
}