using UnityEngine;

namespace EnoPM.BetterVanilla.Components;

public class SettingsTabController : TabController
{
    public GameObject settingsContainer;
    public string categoryId;
    
    public GameObject toggleSettingPrefab;
    public GameObject dropdownSettingPrefab;
    public GameObject sliderSettingPrefab;

    private void Start()
    {
        Plugin.Logger.LogMessage($"{nameof(SettingsTabController)} for category: {categoryId}");
        ModSettings.InitUi(this);
    }

    public ToggleSettingItem CreateToggleOption()
    {
        return Instantiate(toggleSettingPrefab, settingsContainer.transform).GetComponent<ToggleSettingItem>();
    }
    
    public DropdownSettingItem CreateDropdownOption()
    {
        return Instantiate(dropdownSettingPrefab, settingsContainer.transform).GetComponent<DropdownSettingItem>();
    }

    public SliderSettingItem CreateSliderOption()
    {
        return Instantiate(sliderSettingPrefab, settingsContainer.transform).GetComponent<SliderSettingItem>();
    }
}