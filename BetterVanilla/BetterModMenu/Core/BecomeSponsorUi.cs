using TMPro;
using UnityEngine;

namespace BetterVanilla.BetterModMenu.Core;

public sealed class BecomeSponsorUi : MonoBehaviour
{
    public TextMeshProUGUI descriptionText = null!;
    public TextMeshProUGUI buttonText = null!;
    
    public void OnButtonClicked()
    {
        Application.OpenURL("https://buymeacoffee.com/enopm");
    }
}