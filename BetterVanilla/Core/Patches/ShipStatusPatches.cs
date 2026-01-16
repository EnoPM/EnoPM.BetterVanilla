using BetterVanilla.Core.Data;
using BetterVanilla.Core.Helpers;
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

    [HarmonyPrefix, HarmonyPatch(nameof(ShipStatus.Begin))]
    private static bool BeginPrefix(ShipStatus __instance)
    {
        var assignation = new CustomTasksAssignation(__instance);
        
        TaskMover.Refresh();
        
        assignation.Begin();

        PlayerControl.LocalPlayer.cosmetics.SetAsLocalPlayer();

        return false;
    }

    [HarmonyPostfix, HarmonyPatch(nameof(ShipStatus.Awake))]
    private static void AwakePostfix(ShipStatus __instance)
    {
        BetterPolusUtils.AdjustPolusMap(__instance);
    }
}