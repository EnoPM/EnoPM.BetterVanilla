using HarmonyLib;

namespace BetterVanilla.Cosmetics.Core.Patches;

[HarmonyPatch(typeof(CosmeticsCache))]
internal static class CosmeticsCachePatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(CosmeticsCache.GetHat))]
    private static bool GetHatPrefix(string id, ref HatViewData __result)
    {
        return CosmeticsContext.Hats.TryGetViewData(id, out __result);
    }
    
    [HarmonyPatch, HarmonyPatch(nameof(CosmeticsCache.GetVisor))]
    private static bool GetVisorPrefix(string id, ref VisorViewData __result)
    {
        return CosmeticsContext.Visors.TryGetViewData(id, out __result);
    }
}