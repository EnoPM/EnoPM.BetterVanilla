using BetterVanilla.Components;
using BetterVanilla.Cosmetics;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(PlayerPurchasesData))]
internal static class PlayerPurchasesDataPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerPurchasesData.GetPurchase))]
    private static void GetPurchasePostfix(string itemKey, string bundleKey, ref bool __result)
    {
        if (bundleKey == "BetterVanilla")
        {
            Ls.LogInfo($"Checking custom cosmetic unlocked '{itemKey}'");
            __result = CosmeticsManager.IsUnlocked(itemKey);
            return;
        }
        
        if (!LocalConditions.ShouldUnlockModdedCosmetics())
        {
            return;
        }
        __result = true;
    }
}