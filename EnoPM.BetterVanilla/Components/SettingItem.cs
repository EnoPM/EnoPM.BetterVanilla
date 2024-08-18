using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EnoPM.BetterVanilla.Components;

public abstract class SettingItem : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    private readonly List<Action> _onValueChangedActions = [];

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

    public abstract void SetEditable(bool isEditable);
}