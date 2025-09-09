using HarmonyLib;

namespace BetterVanilla.Cosmetics.Core.Patches;

[HarmonyPatch(typeof(CosmeticsLayer))]
internal static class CosmeticsLayerPatches
{
    

    [HarmonyPostfix, HarmonyPatch(nameof(CosmeticsLayer.SetVisor), typeof(string), typeof(int))]
    private static void SetVisorPostfix(CosmeticsLayer __instance, string visorId, int color)
    {
        if (__instance.visor == null || __instance.visor.visorData == null || __instance.visor.visorData.ProductId != visorId)
        {
            return;
        }
        __instance.visor.PopulateAsset();
    }
    
    [HarmonyPostfix, HarmonyPatch(nameof(CosmeticsLayer.SetVisor), typeof(VisorData), typeof(int))]
    private static void SetVisorPostfix(CosmeticsLayer __instance, VisorData visorData, int color)
    {
        if (visorData == null || __instance.visor == null || __instance.visor.visorData == null || __instance.visor.visorData.ProductId != visorData.ProductId)
        {
            return;
        }
        __instance.visor.PopulateAsset();
    }

    private static void PopulateAsset(this VisorLayer visor)
    {
        if (CosmeticsManager.Visors.TryGetViewData(visor.visorData.ProductId, out var viewData))
        {
            CosmeticsManager.Visors.PopulateParentFromAsset(visor, viewData);
        }
        else
        {
            visor.PopulateFromViewData();
        }
    }
}