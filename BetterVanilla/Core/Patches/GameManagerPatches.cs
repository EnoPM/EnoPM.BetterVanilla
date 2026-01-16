using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(GameManager))]
internal static class GameManagerPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(GameManager.CheckEndGameViaTasks))]
    private static bool CheckEndGameViaTasksPrefix(GameManager __instance)
    {
        return !LocalConditions.ShouldDisableGameEndRequirement();
    }
}