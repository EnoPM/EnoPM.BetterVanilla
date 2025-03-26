using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(LogicGameFlowNormal))]
internal static class LogicGameFlowNormalPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(LogicGameFlowNormal.CheckEndCriteria))]
    [HarmonyPatch(nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    private static bool ShouldCheckForEndGame()
    {
        return !LocalConditions.ShouldDisableGameEndRequirement();
    }
}