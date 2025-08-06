using BetterVanilla.Core;
using UnityEngine;

namespace BetterVanilla.Components.BaseComponents;

public abstract class BaseWindowUi : MonoBehaviour
{
    private readonly UiInteractionBlocker _blocker = new();

    public virtual void Show()
    {
        gameObject.SetActive(true);
        _blocker.Block();
    }

    public virtual void Hide()
    {
        _blocker.Unblock();
        gameObject.SetActive(false);
    }
}