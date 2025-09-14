using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(MainMenuManager))]
internal static class MainMenuManagerPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(MainMenuManager.OpenCreateGame))]
    internal static bool OpenCreateGamePrefix(MainMenuManager __instance)
    {
        if (__instance.animating)
            return false;
        if (!DestroyableSingleton<AccountManager>.Instance.CanPlayOnline())
        {
            AmongUsClient.Instance.LastDisconnectReason = DisconnectReasons.NotAuthorized;
            DestroyableSingleton<DisconnectPopup>.Instance.Show();
        }
        else
        {
            ControllerManager.Instance.SetCurrentSelected(null);
            __instance.StartCoroutine(__instance.ShowCreateGameCo());
        }

        return false;
    }
}