using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using BetterVanilla.Options.Components.Controllers;
using UnityEngine;

namespace BetterVanilla.Options.Components;

public abstract class BaseOptionUi : MonoBehaviour
{
    public OptionLabelUi label = null!;
    public LockOverlayUi lockOverlay = null!;

    public OptionLabelUi Label => label;

    public void SetLabel(string labelText)
    {
        if (!label)
        {
            this.StartCoroutine(CoSetLabel(labelText));
        }
        else
        {
            label.SetLabel(labelText);
        }
    }

    private IEnumerator CoSetLabel(string labelText)
    {
        while (!label)
        {
            yield return new WaitForEndOfFrame();
        }
        label.SetLabel(labelText);
    }

    public virtual bool MatchSearch(string searchText)
    {
        return label.labelText.text
            .ToLowerInvariant()
            .Contains(searchText);
    }

    public virtual void SetActive(bool active) => gameObject.SetActive(active);

    public abstract void RefreshVisibility();
}