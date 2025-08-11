using BetterVanilla.Options.Components;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options.Core.Local;

public sealed class EnumLocalOption(string key, string title, object defaultValue) : EnumSerializableOption(key, title, defaultValue), ILocalOption<EnumOptionUi>
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
    
    public void RefreshUiLock()
    {
        if (UiOption == null || UiOption.lockOverlay == null) return;
        var isLocked = IsLocked();
        UiOption.lockOverlay.SetActive(isLocked);
        if (!isLocked) return;
        UiOption.lockOverlay.SetLockedText(LockedText ?? "Locked");
    }

    public void RefreshUiVisibility()
    {
        if (UiOption == null) return;
        UiOption.IsHidden = IsHidden();
    }
}