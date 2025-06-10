using HarmonyLib;

namespace BetterVanilla.Cosmetics.Core.Patches;

[HarmonyPatch(typeof(HatManager))]
internal static class HatManagerPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(HatManager.Initialize))]
    private static void InitializePostfix(HatManager __instance)
    {
        CosmeticsPlugin.Instance.ProcessUnregisteredCosmetics();
    }
}