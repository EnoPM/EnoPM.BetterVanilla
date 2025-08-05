using System;
using BetterVanilla.Options.Core.Local;
using UnityEngine.UI;

namespace BetterVanilla.Options.Components;

public sealed class ToggleOptionUi : BaseOptionUi
{
    public Toggle toggle = null!;
    
    private BoolLocalOption? SerializableOption { get; set; }

    private void Awake()
    {
        toggle.onValueChanged.AddListener(new Action<bool>(OnToggleValueChanged));
    }

    private void OnToggleValueChanged(bool value)
    {
        if (SerializableOption == null) return;
        SerializableOption.Value = value;
    }
    
    public void SetOption(BoolLocalOption option)
    {
        SerializableOption = option;
        SerializableOption.SetUiOption(this);
        SerializableOption.RefreshUiOption();
    }

    public void SetValueWithoutNotify(bool value)
    {
        toggle.SetIsOnWithoutNotify(value);
    }
    
    private void Update()
    {
        SerializableOption?.RefreshUiLock();
    }
    
    public override void RefreshVisibility()
    {
        SerializableOption?.RefreshUiVisibility();
    }
}