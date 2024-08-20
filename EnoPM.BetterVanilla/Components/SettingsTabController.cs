using System;
using EnoPM.BetterVanilla.Core.Settings;
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
        ModSettings.OnSettingsTabControllerReady(this);
    }

    private void Update()
    {
        CustomSetting.CheckLockedSettings(categoryId);
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