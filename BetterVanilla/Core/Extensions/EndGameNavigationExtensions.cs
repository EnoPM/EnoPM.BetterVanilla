using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Components;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class EndGameNavigationExtensions
{
    public static void SetupPlayAgain(this EndGameNavigation navigation)
    {
        if (!BetterVanillaManager.Instance.LocalOptions.AutoPlayAgain.Value)
        {
            return;
        }
        navigation.StartCoroutine(navigation.CoPlayAgain());
    }

    private static IEnumerator CoPlayAgain(this EndGameNavigation navigation)
    {
        while (!navigation.PlayAgainButton.enabled)
        {
            yield return new WaitForEndOfFrame();
        }

        var continueButton = navigation.ContinueButton.GetComponentInChildren<PassiveButton>();
        if (continueButton && continueButton.enabled)
        {
            continueButton.OnClick.Invoke();
        }
        
        while (!navigation.PlayAgainButton.enabled)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(3f);

        var rejoinButton = navigation.PlayAgainButton.gameObject.GetComponent<PassiveButton>();
        if (rejoinButton && rejoinButton.enabled)
        {
            rejoinButton.OnClick.Invoke();
        }
    }
}