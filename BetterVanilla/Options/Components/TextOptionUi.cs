using System;
using BetterVanilla.Options.Core.Local;
using BetterVanilla.Options.Core.Serialization;
using TMPro;

namespace BetterVanilla.Options.Components;

public sealed class TextOptionUi : BaseOptionUi
{
    public TMP_InputField textField = null!;
    public TextMeshProUGUI placeholder = null!;
    
    private TextLocalOption? SerializableOption { get; set; }

    private void Awake()
    {
        textField.onValueChanged.AddListener(new Action<string>(OnTextValueChanged));
    }

    private void OnTextValueChanged(string text)
    {
        if (SerializableOption == null) return;
        SerializableOption.Value = text;
    }

    public void SetOption(TextLocalOption option)
    {
        SerializableOption = option;
        SerializableOption.SetUiOption(this);
        SerializableOption.RefreshUiOption();
    }

    public void SetValueWithoutNotify(string value)
    {
        textField.SetTextWithoutNotify(value);
    }

    public void SetValueText(string text) => textField.SetText(text);
    public void SetPlaceholderText(string text) => placeholder.SetText(text);
    
    private void Update()
    {
        SerializableOption?.RefreshUiLock();
    }
    
    public override void RefreshVisibility()
    {
        SerializableOption?.RefreshUiVisibility();
    }
}