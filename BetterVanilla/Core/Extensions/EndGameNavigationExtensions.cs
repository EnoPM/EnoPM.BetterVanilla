using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class EndGameNavigationExtensions
{
    public static void SetupPlayAgain(this EndGameNavigation navigation)
    {
        if (!LocalConditions.ShouldAutoPlayAgain()) return;
        navigation.StartCoroutine(navigation.CoPlayAgain());
    }

    private static IEnumerator CoPlayAgain(this EndGameNavigation navigation)
    {
        while (!navigation.ContinueButton.activeSelf)
        {
            yield return new WaitForEndOfFrame();
        }
        
        var continueButton = navigation.ContinueButton.GetComponentInChildren<PassiveButton>();
        continueButton.enabled = false;
        
        yield return new WaitForSeconds(3f);
        
        if (continueButton != null)
        {
            continueButton.OnClick.Invoke();
        }
        
        while (!navigation.PlayAgainButton.enabled)
        {
            yield return new WaitForEndOfFrame();
        }
        
        var playAgainButton = navigation.PlayAgainButton.gameObject.GetComponent<PassiveButton>();
        playAgainButton.enabled = false;

        yield return new WaitForSeconds(3f);
        
        if (playAgainButton != null)
        {
            playAgainButton.OnClick.Invoke();
        }
    }
}