using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options.Core.Local;
using BetterVanilla.Options.Core.Serialization;
using TMPro;

namespace BetterVanilla.Options.Components;

public sealed class EnumOptionUi : BaseOptionUi
{
    public TMP_Dropdown dropdown = null!;
    
    private EnumLocalOption? SerializableOption { get; set; }

    public void SetOption(EnumLocalOption option)
    {
        SerializableOption = option;
        SerializableOption.SetUiOption(this);
        SerializableOption.RefreshUiOption();
        dropdown.onValueChanged.AddListener(new Action<int>(OnDropdownValueChanged));
    }

    public void SetValueIndex(int index)
    {
        dropdown.value = index;
    }

    public void SetValueIndexWithoutNotify(int index)
    {
        dropdown.SetValueWithoutNotify(index);
    }

    public void SetOptions(IEnumerable<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options.ToIl2CppList());
    }

    private void OnDropdownValueChanged(int key)
    {
        var value = dropdown.options[key].text;
        if (SerializableOption == null) return;
        var match = SerializableOption.AllowedValues.First(x => x.Value == value).Key;
        SerializableOption.Value = match;
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