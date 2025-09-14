using BetterVanilla.Core.Data;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(GameOptionsMapPicker))]
internal static class GameOptionsMapPickerPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(GameOptionsMapPicker.SelectMap), typeof(int))]
    private static void SelectMapPostfix(int mapId)
    {
        MapTasks.Current?.RefreshOptions();
    }
}