using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(LogicGameFlowNormal))]
internal static class LogicGameFlowNormalPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(LogicGameFlowNormal.CheckEndCriteria)), HarmonyPatch(nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    private static bool ShouldCheckForEndGame() => ModSettings.Local.DisableEndGameChecks.IsLocked() || !ModSettings.Local.DisableEndGameChecks;
}