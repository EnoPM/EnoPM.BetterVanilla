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
        if (CosmeticsContext.Visors.TryGetViewData(visorId, out var viewData))
        {
            CosmeticsContext.Visors.PopulateParentFromAsset(__instance.visor, viewData);
        }
    }
    
    [HarmonyPostfix, HarmonyPatch(nameof(CosmeticsLayer.SetVisor), typeof(VisorData), typeof(int))]
    private static void SetVisorPostfix(CosmeticsLayer __instance, VisorData visorData, int color)
    {
        if (visorData == null || __instance.visor == null || __instance.visor.visorData == null || __instance.visor.visorData.ProductId != visorData.ProductId)
        {
            return;
        }
        if (CosmeticsContext.Visors.TryGetViewData(visorData.ProductId, out var viewData))
        {
            CosmeticsContext.Visors.PopulateParentFromAsset(__instance.visor, viewData);
        }
    }
}