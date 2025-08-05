using BetterVanilla.Core;
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

    public void RefreshUiVisibility()
    {
        if (UiOption == null) return;
        var isHidden = IsHidden();
        UiOption.SetActive(!isHidden);
    }

    public void RefreshUiLock()
    {
        if (UiOption == null || UiOption.lockOverlay == null) return;
        var isLocked = IsLocked();
        UiOption.lockOverlay.SetActive(isLocked);
        if (!isLocked) return;
        UiOption.lockOverlay.SetLockedText(LockedText ?? "Locked");
    }
}