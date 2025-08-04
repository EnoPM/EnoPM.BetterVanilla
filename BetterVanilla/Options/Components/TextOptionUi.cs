using System;
using BetterVanilla.Options.Core.Serialization;
using TMPro;

namespace BetterVanilla.Options.Components;

public sealed class TextOptionUi : BaseOptionUi
{
    public TMP_InputField textField = null!;
    public TextMeshProUGUI placeholder = null!;
    
    private TextSerializableOption? SerializableOption { get; set; }

    private void Awake()
    {
        textField.onValueChanged.AddListener(new Action<string>(OnTextValueChanged));
    }

    private void OnTextValueChanged(string text)
    {
        if (SerializableOption == null) return;
        SerializableOption.Value = text;
    }

    public void SetOption(TextSerializableOption option)
    {
        SerializableOption = option;
        SetLabel(option.Title);
        textField.SetTextWithoutNotify(SerializableOption.Value);
    }

    public void SetValueText(string text) => textField.SetText(text);
    public void SetPlaceholderText(string text) => placeholder.SetText(text);
}