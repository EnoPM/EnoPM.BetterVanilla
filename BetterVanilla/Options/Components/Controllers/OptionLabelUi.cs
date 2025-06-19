using TMPro;
using UnityEngine;

namespace BetterVanilla.Options.Components.Controllers;

public sealed class OptionLabelUi : MonoBehaviour
{
    public TextMeshProUGUI labelText = null!;

    public void SetLabel(string text)
    {
        labelText.SetText(text);
    }
}