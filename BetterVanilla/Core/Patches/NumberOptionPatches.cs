using AmongUs.GameOptions;
using BetterVanilla.Core.Extensions;
using HarmonyLib;
using UnityEngine;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(NumberOption))]
internal static class NumberOptionPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(NumberOption.Initialize))]
    private static bool InitializePrefix(NumberOption __instance)
    {
        return __instance.Title != StringNames.None || __instance.floatOptionName != FloatOptionNames.Invalid || __instance.intOptionName != Int32OptionNames.Invalid;
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(NumberOption.UpdateValue))]
    private static bool UpdateValuePrefix(NumberOption __instance)
    {
        return __instance.CustomUpdateValue();
    }

    [HarmonyPrefix, HarmonyPatch(nameof(NumberOption.Increase))]
    private static bool IncreasePrefix(NumberOption __instance)
    {
        __instance.BetterIncrease();
        return false;
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(NumberOption.Decrease))]
    private static bool DecreasePrefix(NumberOption __instance)
    {
        __instance.BetterDecrease();
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(NumberOption.AdjustButtonsActiveState))]
    private static bool AdjustButtonsActiveStatePrefix(NumberOption __instance)
    {
        __instance.MinusBtn.SetInteractable(!Mathf.Approximately(__instance.Value, __instance.ValidRange.min));
        __instance.PlusBtn.SetInteractable(!Mathf.Approximately(__instance.Value, __instance.ValidRange.max));
        return false;
    }
}