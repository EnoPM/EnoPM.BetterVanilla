using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(Constants))]
internal static class ConstantsPatches
{
    private const int DisableServerAuthorityFlag = 25;
    
    [HarmonyPostfix, HarmonyPatch(nameof(Constants.GetBroadcastVersion))]
    private static void GetBroadcastVersionPostfix(ref int __result)
    {
        if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame) return;

        var revision = __result % 50;
        if (revision < DisableServerAuthorityFlag)
        {
            __result += DisableServerAuthorityFlag;
        }
    }

    [HarmonyPrefix, HarmonyPatch(nameof(Constants.IsVersionModded))]
    private static bool IsVersionModdedPrefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}