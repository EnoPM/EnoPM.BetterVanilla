using BetterVanilla.Options.Components;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options.Core.Local;

public sealed class NumberLocalOption : NumberSerializableOption, ILocalOption<NumberOptionUi>
{
    public NumberLocalOption(string key,
        string title,
        float defaultValue,
        float incrementValue,
        float minValue,
        float maxValue,
        string valuePrefix = "",
        string valueSuffix = "") : base(key,
        title,
        defaultValue,
        incrementValue,
        minValue,
        maxValue,
        valuePrefix,
        valueSuffix)
    {
        MinValueChanged += RefreshUiOption;
        MaxValueChanged += RefreshUiOption;
    }

    public NumberOptionUi? UiOption { get; private set; }

    public void SetUiOption(NumberOptionUi option)
    {
        UiOption = option;
    }

    public void RefreshUiOption()
    {
        if (UiOption == null) return;
        UiOption.SetLabel(Title);
        UiOption.SetMinValue(MinValue);
        UiOption.SetMaxValue(MaxValue);
        UiOption.SetValueWithoutNotify(Value);
        UiOption.SetValueText(GetValueAsString());
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