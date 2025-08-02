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
        sponsorText.SetText(text);
    }
    
    public void SetSponsorText(string text)
    {
        roleAndTaskProgressionText.SetText(text);
    }
}