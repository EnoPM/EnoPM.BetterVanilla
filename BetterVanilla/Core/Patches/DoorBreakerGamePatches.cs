using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Extensions;
using HarmonyLib;
using UnityEngine;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(DoorBreakerGame))]
internal static class DoorBreakerGamePatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(DoorBreakerGame.Start))]
    private static bool StartPrefix(DoorBreakerGame __instance)
    {
        if (!BetterVanillaManager.Instance.LocalOptions.AutoOpenDoors.Value)
        {
            return true;
        }
        foreach (var button in __instance.Buttons)
        {
            button.color = Color.gray;
            button.GetComponent<PassiveButton>().enabled = false;
        }
        var buttons = __instance.Buttons
            .ToList()
            .PickRandom(4);
        foreach (var button in buttons)
        {
            if (button.flipX) continue;
            button.color = Color.white;
            button.flipX = true;
        }

        __instance.StartCoroutine(__instance.CoSwitchButtons(buttons));
        
        return false;
    }

    private static float GetWaitTime()
    {
        var option = BetterVanillaManager.Instance.LocalOptions.DoorSwitchInterval;
        var value = option.ParseValue(DoorOpenDelay.Ms5);
        return value switch
        {
            DoorOpenDelay.Ms1 => 0.1f,
            DoorOpenDelay.Ms2 => 0.2f,
            DoorOpenDelay.Ms3 => 0.3f,
            DoorOpenDelay.Ms4 => 0.4f,
            DoorOpenDelay.Ms5 => 0.5f,
            _ => 1f
        };
    }

    private static IEnumerator CoSwitchButtons(this DoorBreakerGame doorGame, List<SpriteRenderer> buttons)
    {
        foreach (var button in buttons)
        {
            yield return new WaitForSeconds(GetWaitTime());
            doorGame.FlipSwitch(button);
        }
    }
}