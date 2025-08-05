using BetterVanilla.Options.Components;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options.Core.Local;

public sealed class EnumLocalOption(string key, string title, object defaultValue) : EnumSerializableOption(key, title, defaultValue)
{
    public EnumOptionUi? UiOption { get; private set; }

    public void SetUiOption(EnumOptionUi option)
    {
        UiOption = option;
    }

    public void RefreshUiOption()
    {
        if (UiOption == null) return;
        UiOption.SetLabel(Title);
        UiOption.SetOptions(AllowedValues.Values);
        UiOption.SetValueIndexWithoutNotify(ValueIndex);
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