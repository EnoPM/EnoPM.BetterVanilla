using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(Constants))]
internal static class ConstantsPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(Constants.GetBroadcastVersion))]
    private static void GetBroadcastVersionPostfix(ref int __result)
    {
        __result += 25;
    }
}