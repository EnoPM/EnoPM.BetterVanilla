using BetterVanilla.Options.Components;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options.Core.Local;

public sealed class BoolLocalOption(string key, string title, bool defaultValue) : BoolSerializableOption(key, title, defaultValue)
{
    public ToggleOptionUi? UiOption { get; private set; }

    public void SetUiOption(ToggleOptionUi option)
    {
        UiOption = option;
    }

    public void RefreshUiOption()
    {
        if (UiOption == null) return;
        UiOption.SetLabel(Title);
        UiOption.SetValueWithoutNotify(Value);
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