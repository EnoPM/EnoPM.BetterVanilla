using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(ShipStatus))]
internal static class ShipStatusPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(ShipStatus.Start))]
    private static void StartPostfix(ShipStatus __instance)
    {
        GameEventManager.TriggerGameStarted();
    }

    [HarmonyPrefix, HarmonyPatch(nameof(ShipStatus.OnDestroy))]
    private static void OnDestroyPrefix(ShipStatus __instance)
    {
        GameEventManager.TriggerGameEnded();
    }
}