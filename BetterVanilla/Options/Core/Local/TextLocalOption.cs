using BetterVanilla.Options.Components;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options.Core.Local;

public sealed class TextLocalOption(string key, string title, string defaultValue)
    : TextSerializableOption(key, title, defaultValue)
{
    public TextOptionUi? UiOption { get; private set; }

    public void SetUiOption(TextOptionUi option)
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