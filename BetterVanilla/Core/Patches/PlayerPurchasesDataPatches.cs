using BetterVanilla.Components;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(PlayerPurchasesData))]
internal static class PlayerPurchasesDataPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerPurchasesData.GetPurchase))]
    private static void GetPurchasePostfix(ref bool __result)
    {
        if (BetterVanillaManager.Instance.LocalOptions.AllowModdedCosmetics.IsLocked() || !BetterVanillaManager.Instance.LocalOptions.AllowModdedCosmetics.Value)
        {
            return;
        }
        __result = true;
    }
}