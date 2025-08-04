using System;
using System.Linq;
using BetterVanilla.Core;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Options.Core.Serialization;
using TMPro;

namespace BetterVanilla.Options.Components;

public sealed class EnumOptionUi : BaseOptionUi
{
    public TMP_Dropdown dropdown = null!;
    
    private EnumSerializableOption? SerializableOption { get; set; }

    public void SetOption(EnumSerializableOption option)
    {
        SerializableOption = option;
        SetLabel(SerializableOption.Title);
        dropdown.ClearOptions();
        var options = SerializableOption.AllowedValues.Values.ToIl2CppList();
        dropdown.AddOptions(options);
        dropdown.value = SerializableOption.ValueIndex;
        dropdown.onValueChanged.AddListener(new Action<int>(OnDropdownValueChanged));
    }

    private void OnDropdownValueChanged(int key)
    {
        var value = dropdown.options[key].text;
        if (SerializableOption == null) return;
        var match = SerializableOption.AllowedValues.First(x => x.Value == value).Key;
        SerializableOption.Value = match;
    }
}