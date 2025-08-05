using BetterVanilla.Options.Components;
using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options.Core.Local;

public sealed class NumberLocalOption : NumberSerializableOption
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