using TMPro;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class BetterPlayerTexts : MonoBehaviour
{
    public TextMeshPro roleAndTaskProgressionText = null!;
    public TextMeshPro sponsorText = null!;
    
    public bool IsReady => roleAndTaskProgressionText != null && sponsorText != null;

    public void SetMainText(string text)
    {
        sponsorText.gameObject.SetActive(!string.IsNullOrWhiteSpace(text));
        sponsorText.SetText(text);
    }
    
    public void SetSponsorText(string text)
    {
        roleAndTaskProgressionText.gameObject.SetActive(!string.IsNullOrWhiteSpace(text));
        roleAndTaskProgressionText.SetText(text);
    }
}