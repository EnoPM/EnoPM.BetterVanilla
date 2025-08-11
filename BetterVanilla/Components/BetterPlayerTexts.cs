using BetterVanilla.Core.Data;
using BetterVanilla.Core.Helpers;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Components;

public sealed class BetterPlayerTexts : MonoBehaviour
{
    public TextMeshPro roleAndTaskProgressionText = null!;
    public TextMeshPro sponsorText = null!;
    public TextMeshPro handshakeText = null!;
    
    public bool IsReady => roleAndTaskProgressionText != null && sponsorText != null && handshakeText != null;

    public void SetMainText(string text)
    {
        roleAndTaskProgressionText.SetText(text);
    }
    
    public void SetSponsorText(string text)
    {
        sponsorText.SetText(text);
    }

    public void SetHandshakeText(BetterVanillaHandshake? handshake)
    {
        if (handshake == null)
        {
            handshakeText.SetText(string.Empty);
            return;
        }
        var color = Color.green;
        if (BetterVanillaHandshake.Local.Version != handshake.Version)
        {
            color = Color.red;
        }
        else if (BetterVanillaHandshake.Local.Guid != handshake.Guid)
        {
            color = ColorUtils.FromHex("#FF8800");
        }
        handshakeText.SetText(ColorUtils.ColoredString(color, $"v{handshake.Version}", false));
    }
}