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
        label.SetLabel(labelText);
    }

    public virtual bool MatchSearch(string searchText)
    {
        return label.labelText.text
            .ToLowerInvariant()
            .Contains(searchText);
    }

    public virtual void SetActive(bool active) => gameObject.SetActive(active);
}