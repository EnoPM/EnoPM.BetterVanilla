using AmongUs.Data;
using AmongUs.GameOptions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(HostLocalGameButton))]
internal static class HostLocalGameButtonPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(HostLocalGameButton.OnClick))]
    private static bool OnClickPrefix(HostLocalGameButton __instance)
    {
        GameOptionsManager.Instance.SwitchGameMode(GameModes.Normal);
        if (__instance.NetworkMode == NetworkModes.FreePlay)
        {
            if (!NameTextBehaviour.IsValidName(DataManager.Player.Customization.Name))
            {
                DataManager.Player.Customization.Name = "";
                DataManager.Player.Save();
            }
        }
        else
        {
            if (!DestroyableSingleton<MatchMaker>.Instance.Connecting<HostLocalGameButton>(__instance))
                return false;
        }
        Logger.GlobalInstance.Info("Hosting a local game");
        __instance.StartCoroutine(__instance.CoStartGame());
        return false;
    }
}