using AmongUs.Data;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(AmongUsClient))]
internal static class AmongUsClientPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(AmongUsClient.CheckOnlinePermissions))]
    private static bool CheckOnlinePermissionsPrefix(AmongUsClient __instance, Il2CppSystem.Action success, Il2CppSystem.Action failure, Il2CppSystem.Action loadingCallback, bool checkOnline)
    {
        if (checkOnline && !DestroyableSingleton<AccountManager>.Instance.CanPlayOnline())
        {
            __instance.LastDisconnectReason = DisconnectReasons.NotAuthorized;
            DestroyableSingleton<DisconnectPopup>.Instance.Show();
        }
        else
        {
            if (success == null) return false;
            success.Invoke();
        }

        return false;
    }
}