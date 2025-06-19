using TMPro;

namespace BetterVanilla.Options.Components;

public sealed class TextOptionUi : BaseOptionUi
{
    public TMP_InputField textField = null!;
    public TextMeshProUGUI placeholder = null!;

    public void SetValueText(string text) => textField.SetText(text);
    public void SetPlaceholderText(string text) => placeholder.SetText(text);
}