using TMPro;
using UnityEngine;

namespace BetterVanilla.Options.Components.Controllers;

public sealed class LockOverlayUi : MonoBehaviour
{
    public TextMeshProUGUI text = null!;
    
    public void SetActive(bool active) => gameObject.SetActive(active);

    public void SetLockedText(string lockedText)
    {
        if (text == null) return;
        text.SetText(lockedText);
    }
}