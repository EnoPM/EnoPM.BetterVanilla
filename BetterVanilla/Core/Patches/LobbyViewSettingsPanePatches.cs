using BetterVanilla.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(LobbyViewSettingsPane))]
internal static class LobbyViewSettingsPanePatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(LobbyViewSettingsPane.DrawNormalTab))]
    private static bool DrawNormalTabPrefix(LobbyViewSettingsPane __instance)
    {
        __instance.DrawBetterNormalTab();
        return false;
    }
}