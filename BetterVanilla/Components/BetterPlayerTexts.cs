using TMPro;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class BetterPlayerTexts : MonoBehaviour
{
    public TextMeshPro roleAndTaskProgressionText = null!;
    public TextMeshPro sponsorText = null!;

    public TextMeshPro MainText => roleAndTaskProgressionText;
    public TextMeshPro SponsorText => sponsorText;
}