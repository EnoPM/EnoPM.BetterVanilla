using BetterVanilla.Options.Components;
using BetterVanilla.Options.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Options.Core.Local;

public sealed class ColorLocalOption(string key, string title, Color defaultValue) : ColorSerializableOption(key, title, defaultValue)
{
    public ColorOptionUi? UiOption { get; private set; }

    public void SetUiOption(ColorOptionUi option)
    {
        UiOption = option;
    }

    public void RefreshUiOption()
    {
        if (UiOption == null) return;
        UiOption.SetLabel(Title);
        UiOption.SetColor(Value);
    }

    public void RefreshLockAndVisibility()
    {
        if (UiOption == null) return;
        UiOption.lockOverlay.SetActive(IsLocked());
        if (LockedText != null)
        {
            UiOption.lockOverlay.SetLockedText(LockedText);
        }
        UiOption.gameObject.SetActive(!IsHidden());
    }
}