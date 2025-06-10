using HarmonyLib;

namespace BetterVanilla.Cosmetics.Core.Patches;

[HarmonyPatch(typeof(CosmeticsCache))]
internal static class CosmeticsCachePatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(CosmeticsCache.GetHat))]
    private static bool GetHatPrefix(string id, ref HatViewData __result)
    {
        return CosmeticsPlugin.Instance.Hats.TryGetViewData(id, out __result);
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(CosmeticsCache.GetVisor))]
    private static bool GetVisorPrefix(string id, ref VisorViewData __result)
    {
        return CosmeticsPlugin.Instance.Visors.TryGetViewData(id, out __result);
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(CosmeticsCache.GetNameplate))]
    private static bool GetNameplatePrefix(string id, ref NamePlateViewData __result)
    {
        return CosmeticsPlugin.Instance.NamePlates.TryGetViewData(id, out __result);
    }
}