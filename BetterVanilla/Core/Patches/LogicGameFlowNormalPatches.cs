using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(LogicGameFlowNormal))]
internal static class LogicGameFlowNormalPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(LogicGameFlowNormal.CheckEndCriteria))]
    private static bool CheckEndCriteriaPrefix()
    {
        return !LocalConditions.ShouldDisableGameEndRequirement();
    }

    [HarmonyPrefix, HarmonyPatch(nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    private static bool IsGameOverDueToDeathPrefix(ref bool __result)
    {
        if (!LocalConditions.ShouldDisableGameEndRequirement())
        {
            return true;
        }

        __result = false;
        
        return false;
    }
}