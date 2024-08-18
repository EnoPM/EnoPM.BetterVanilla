using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components
{
    public class ModUpdaterController : MonoBehaviour
    {
        public GameObject canvas;
        public Button closeButton;
        public Button installButton;
        public TextMeshProUGUI updateText;
        public RectTransform progressBarContainerRect;
        public RectTransform progressBarRect;
        public TextMeshProUGUI progressBarText;
    }
}
