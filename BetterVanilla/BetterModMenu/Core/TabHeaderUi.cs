using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.BetterModMenu.Core;

public class TabHeaderUi : MonoBehaviour
{
    public Button button = null!;
    public Image icon = null!;
    
    private static Color DisabledColor { get; } = new(1f, 1f, 1f, 0.4f);

    public void SetEnabled(bool value)
    {
        button.interactable = value;
        icon.color = value ? Color.white : DisabledColor;
    }
}