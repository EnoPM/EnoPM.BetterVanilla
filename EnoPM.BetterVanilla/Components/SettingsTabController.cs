using UnityEngine;

namespace EnoPM.BetterVanilla.Components;

public class SettingsTabController : TabController
{
    public GameObject settingsContainer;
    
    public GameObject toggleSettingPrefab;
    public GameObject dropdownSettingPrefab;

    private void Start()
    {
        Plugin.Logger.LogMessage($"{nameof(SettingsTabController)} started!");

        InitOptions();
    }

    private void InitOptions()
    {
        var toggleOption = CreateToggleOption();
        toggleOption.SetTitle("Toggle Option Demo");
        toggleOption.SetValue(false);

        var dropdownOption = CreateDropdownOption();
        dropdownOption.SetTitle("Dropdown Option Demo");
        dropdownOption.SetValue(1);
    }

    private ToggleSettingItem CreateToggleOption()
    {
        return Instantiate(toggleSettingPrefab, settingsContainer.transform).GetComponent<ToggleSettingItem>();
    }
    
    private DropdownSettingItem CreateDropdownOption()
    {
        return Instantiate(dropdownSettingPrefab, settingsContainer.transform).GetComponent<DropdownSettingItem>();
    }
}