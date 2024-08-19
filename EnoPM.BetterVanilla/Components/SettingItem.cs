using System;
using System.Collections.Generic;
using EnoPM.BetterVanilla.Core.Settings;
using TMPro;
using UnityEngine;

namespace EnoPM.BetterVanilla.Components;

public abstract class SettingItem : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    private readonly List<Action> _onValueChangedActions = [];
    
    private CustomSetting Setting { get; set; }

    public void SetSetting(CustomSetting setting)
    {
        Setting = setting;
    }

    public void SetTitle(string title)
    {
        titleText.SetText(title);
    }

    public void AddOnValueChangedListener(Action listener)
    {
        _onValueChangedActions.Add(listener);
    }

    protected void TriggerValueChangedHook()
    {
        foreach (var action in _onValueChangedActions)
        {
            action.Invoke();
        }
    }

    protected void Update()
    {
        Setting?.UiUpdate();
    }
}