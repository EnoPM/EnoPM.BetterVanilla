using System;
using BetterVanilla.Options.Core.Serialization;
using UnityEngine.UI;

namespace BetterVanilla.Options.Components;

public sealed class ToggleOptionUi : BaseOptionUi
{
    public Toggle toggle = null!;
    
    private BoolSerializableOption? SerializableOption { get; set; }

    private void Awake()
    {
        toggle.onValueChanged.AddListener(new Action<bool>(OnToggleValueChanged));
    }

    private void OnToggleValueChanged(bool value)
    {
        if (SerializableOption == null) return;
        SerializableOption.Value = value;
    }
    
    public void SetOption(BoolSerializableOption option)
    {
        SerializableOption = option;
        SetLabel(option.Title);
        toggle.SetIsOnWithoutNotify(option.Value);
    }
}