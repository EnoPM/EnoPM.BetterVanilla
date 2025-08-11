using BetterVanilla.Options.Components;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options.Core.Local;

public sealed class TextLocalOption(string key, string title, string defaultValue, int maxLength)
    : TextSerializableOption(key, title, defaultValue, maxLength), ILocalOption<TextOptionUi>
{
    public TextOptionUi? UiOption { get; private set; }

    public void SetUiOption(TextOptionUi option)
    {
        UiOption = option;
    }

    public void RefreshUiOption()
    {
        if (UiOption == null) return;
        UiOption.textField.characterLimit = MaxLength;
        UiOption.SetLabel(Title);
        UiOption.SetValueWithoutNotify(Value);
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