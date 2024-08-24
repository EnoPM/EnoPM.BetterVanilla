using System;
using BetterVanilla.Components.BaseComponents;
using BetterVanilla.Core.Options;
using UnityEngine.UI;

namespace BetterVanilla.Components.Menu.Settings;

public sealed class ToggleSettingBehaviour : BaseSettingBehaviour
{
    public Toggle toggle;
    
    private BoolLocalOption BoolOption { get; set; }
    
    public override void Initialize(BaseLocalOption option)
    {
        if (option is not BoolLocalOption boolOption)
        {
            throw new Exception($"{nameof(BaseLocalOption)} must be {nameof(BoolLocalOption)}");
        }
        BoolOption = boolOption;
        base.Initialize(option);
    }

    private void Awake()
    {
        toggle.onValueChanged.AddListener(new Action<bool>(OnToggleValueChanged));
    }

    private void OnToggleValueChanged(bool value)
    {
        BoolOption.Value = value;
    }

    public override void UpdateFromOption()
    {
        toggle.isOn = BoolOption.Value;
    }
}