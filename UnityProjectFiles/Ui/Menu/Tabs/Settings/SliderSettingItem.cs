using TMPro;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components
{
    public class SliderSettingItem : SettingItem
    {
        public Slider slider;
        public TextMeshProUGUI valueText;
        public Button decrementButton;
        public Button incrementButton;
    }
}