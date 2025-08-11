using BetterVanilla.Components;
using BetterVanilla.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(EndGameManager))]
internal static class EndGameManagerPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(EndGameManager.ShowButtons))]
    private static void ShowButtonsPostfix(EndGameManager __instance)
    {
        var manager = BetterVanillaManager.Instance;
        manager.AllTeamPreferences.Clear();
        manager.AllForcedTeamAssignments.Clear();
        manager.Menu.Hide();
        __instance.Navigation.SetupPlayAgain();
    }
}