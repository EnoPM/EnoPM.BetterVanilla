using UnityEngine;

namespace BetterVanilla.BetterModMenu.Core;

public class BecomeSponsorUi : MonoBehaviour
{
    public void OnButtonClicked()
    {
        Application.OpenURL("https://buymeacoffee.com/enopm");
    }
}