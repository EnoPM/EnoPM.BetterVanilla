using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.BetterModMenu.Core;

public sealed class AlertUi : MonoBehaviour
{
    public Image backgroundImage = null!;
    public TextMeshProUGUI alertText = null!;
    public Color successColor;
    public Color infoColor;
    public Color warningColor;
    public Color errorColor;
    
    public void Hide() => gameObject.SetActive(false);
    
    public void ShowSuccess(string message)
    {
        gameObject.SetActive(true);
        alertText.SetText(message);
        backgroundImage.color = successColor;
    }
    
    public void ShowInfo(string message)
    {
        gameObject.SetActive(true);
        alertText.SetText(message);
        backgroundImage.color = infoColor;
    }

    public void ShowWarning(string message)
    {
        gameObject.SetActive(true);
        alertText.SetText(message);
        backgroundImage.color = warningColor;
    }

    public void ShowError(string message)
    {
        gameObject.SetActive(true);
        alertText.SetText(message);
        backgroundImage.color = errorColor;
    }
}