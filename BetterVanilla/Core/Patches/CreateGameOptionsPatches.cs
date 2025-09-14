using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(CreateGameOptions))]
internal static class CreateGameOptionsPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(CreateGameOptions.Show))]
    private static bool ShowPrefix(CreateGameOptions __instance)
    {
        if (!DestroyableSingleton<AccountManager>.Instance.CanPlayOnline())
        {
            AmongUsClient.Instance.LastDisconnectReason = DisconnectReasons.NotAuthorized;
            DestroyableSingleton<DisconnectPopup>.Instance.Show();
        }
        else
        {
            __instance.gameObject.SetActive(true);
            __instance.foreground.gameObject.SetActive(true);
            __instance.StartCoroutine(__instance.CoShow());
        }
        return false;
    }
}