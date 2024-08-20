using EnoPM.BetterVanilla.Core;
using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(PlayerPurchasesData))]
public static class PlayerPurchasesDataPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerPurchasesData.GetPurchase))]
    private static void GetPurchasePostfix(ref bool __result)
    {
        if (ModSettings.Local.AllowModdedCosmetics.IsLocked() || !ModSettings.Local.AllowModdedCosmetics) return;
        __result = true;
    }
}