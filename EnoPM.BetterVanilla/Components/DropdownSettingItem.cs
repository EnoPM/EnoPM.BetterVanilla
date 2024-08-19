using System;
using System.Collections.Generic;
using EnoPM.BetterVanilla.Core.Extensions;
using TMPro;
using UnityEngine.Events;

namespace EnoPM.BetterVanilla.Components;

public class DropdownSettingItem : SettingItem
{
    public TMP_Dropdown dropdown;

    private Il2CppSystem.Collections.Generic.List<string> _values = new();

    private void Awake()
    {
        dropdown.onValueChanged.AddListener((UnityAction<int>)OnDropdownValueChanged);
        RefreshOptions();
    }

    public void SetValue(int index)
    {
        dropdown.SetValue(index, false);
    }

    public void SetValue(string value)
    {
        SetValue(_values.IndexOf(value));
    }

    public void SetOptions(List<string> values)
    {
        _values = values.ToIl2Cpp();
        RefreshOptions();
    }

    private void OnDropdownValueChanged(int value)
    {
        SetValue(value);
        TriggerValueChangedHook();
    }

    private void RefreshOptions()
    {
        if (!dropdown) return;
        dropdown.ClearOptions();
        dropdown.AddOptions(_values);
    }
    
    public int GetSettingValue() => dropdown.value;
}