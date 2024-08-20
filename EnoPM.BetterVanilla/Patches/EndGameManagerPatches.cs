using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;
using UnityEngine;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(EndGameManager))]
internal static class EndGameManagerPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(EndGameManager.ShowButtons))]
    private static void ShowButtonsPostfix(EndGameManager __instance)
    {
        RoleManagerPatches.PlayerTeamPreferences.Clear();
        if (!ModSettings.Local.AutoRejoinPreviousLobby) return;
        __instance.Navigation.SetupAutoRejoin();
    }

    private static void SetupAutoRejoin(this EndGameNavigation endgameNavigation)
    {
        endgameNavigation.StartCoroutine(CoWaitAndRejoin(endgameNavigation));
    }
    
    private static IEnumerator CoWaitAndRejoin(EndGameNavigation endGameNavigation)
    {
        while (!endGameNavigation.PlayAgainButton.enabled)
        {
            yield return new WaitForEndOfFrame();
        }

        var continueButton = endGameNavigation.ContinueButton.GetComponentInChildren<PassiveButton>();
        if (continueButton && continueButton.enabled)
        {
            continueButton.OnClick.Invoke();
        }
        
        while (!endGameNavigation.PlayAgainButton.enabled)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(3f);

        var rejoinButton = endGameNavigation.PlayAgainButton.gameObject.GetComponent<PassiveButton>();
        if (rejoinButton && rejoinButton.enabled)
        {
            rejoinButton.OnClick.Invoke();
        }
    }
}