using BetterVanilla.Core;
using UnityEngine;

namespace BetterVanilla.Components.BaseComponents;

public abstract class BaseWindowUi : MonoBehaviour
{
    private readonly PassiveButtonsBlocker _blocker = new();

    public void Show()
    {
        gameObject.SetActive(true);
        _blocker.Block();
    }

    public void Hide()
    {
        _blocker.Unblock();
        gameObject.SetActive(false);
    }
}