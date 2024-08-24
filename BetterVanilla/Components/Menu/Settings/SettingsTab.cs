using System;
using System.Collections.Generic;
using BetterVanilla.Components.BaseComponents;
using BetterVanilla.Core.Options;
using UnityEngine;

namespace BetterVanilla.Components.Menu.Settings;

public sealed class SettingsTab : MonoBehaviour
{
    public GameObject settingsContainer;
    public GameObject toggleSettingPrefab;
    public GameObject numberSettingPrefab;
    public GameObject stringSettingPrefab;

    public readonly List<BaseSettingBehaviour> AllSettings = []; 

    private void Start()
    {
        foreach (var category in LocalCategory.AllCategories)
        {
            foreach (var option in category.AllOptions)
            {
                var setting = CreateSetting(option);
                setting.Initialize(option);
                AllSettings.Add(setting);
            }
        }
    }

    private void Update()
    {
        foreach (var setting in AllSettings)
        {
            setting.gameObject.SetActive(!setting.Option.IsLocked());
        }
    }

    private BaseSettingBehaviour CreateSetting(BaseLocalOption option)
    {
        if (option is FloatLocalOption or IntLocalOption)
        {
            return Instantiate(numberSettingPrefab, settingsContainer.transform).GetComponent<NumberSettingBehaviour>();
        }
        if (option is BoolLocalOption)
        {
            return Instantiate(toggleSettingPrefab, settingsContainer.transform).GetComponent<ToggleSettingBehaviour>();
        }
        if (option is StringLocalOption)
        {
            return Instantiate(stringSettingPrefab, settingsContainer.transform).GetComponent<StringSettingBehaviour>();
        }
        throw new Exception($"Unsupported local option type for option {option.Name}");
    }
}