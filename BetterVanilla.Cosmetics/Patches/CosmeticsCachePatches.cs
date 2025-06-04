using HarmonyLib;

namespace BetterVanilla.Cosmetics.Patches;

[HarmonyPatch(typeof(CosmeticsCache))]
internal static class CosmeticsCachePatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(CosmeticsCache.GetHat))]
    private static bool GetHatPrefix(string id, ref HatViewData __result)
    {
        return !CosmeticsManager.HatViewDataCache.TryGetValue(id, out __result);
    }
}