using AmongUs.Data;
using EnoPM.BetterVanilla.Core;
using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(EndGameResult))]
internal static class EndGameResultPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(EndGameResult.Create))]
    private static void CreatePostfix(ref EndGameResult __result)
    {
        var oldXp = __result.XpGrantResult;
        if (oldXp == null || DataManager.Player.Stats.Level < oldXp.MaxLevel)
        {
            return;
        }
        __result = XpManager.GetModdedEndGameResult(__result);
    }
}