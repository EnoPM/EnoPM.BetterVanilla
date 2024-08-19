using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public class ToggleSettingItem : SettingItem
{
    public Toggle toggle;

    private void Awake()
    {
        toggle.onValueChanged.AddListener((UnityAction<bool>)OnToggleValueChanged);
    }

    public void SetValue(bool value)
    {
        toggle.Set(value, false);
    }

    private void OnToggleValueChanged(bool value)
    {
        SetValue(value);
        TriggerValueChangedHook();
    }

    public bool GetSettingValue() => toggle.isOn;
}