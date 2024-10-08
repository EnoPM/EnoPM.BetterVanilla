﻿using EnoPM.BetterVanilla.Components;
using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(HudManager))]
internal static class HudManagerPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(HudManager.Start))]
    private static void StartPostfix(HudManager __instance)
    {
        __instance.gameObject.AddComponent<ZoomBehaviour>();
        __instance.gameObject.AddComponent<CustomButtonsManager>();
    }
}