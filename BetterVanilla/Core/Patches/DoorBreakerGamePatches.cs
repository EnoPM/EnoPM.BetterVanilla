using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
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

    private static IEnumerator CoSwitchButtons(this DoorBreakerGame doorGame, List<SpriteRenderer> buttons)
    {
        foreach (var button in buttons)
        {
            yield return new WaitForSeconds(0.25f);
            doorGame.FlipSwitch(button);
        }
    }
}