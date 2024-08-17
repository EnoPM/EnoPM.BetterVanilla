using TMPro;
using UnityEngine;

namespace EnoPM.BetterVanilla.Components;

public class SettingItem : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    public void SetTitle(string title)
    {
        titleText.SetText(title);
    }
}