using EnoPM.BetterVanilla.Core;
using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(PlayerPurchasesData))]
public static class PlayerPurchasesDataPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerPurchasesData.GetPurchase))]
    private static void GetPurchasePostfix(ref bool __result)
    {
        if (!ModConfigs.IsExperimental) return;
        __result = true;
    }
}