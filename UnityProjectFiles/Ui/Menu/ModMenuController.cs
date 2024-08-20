using UnityEngine;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components
{
    public class ModMenuController : MonoBehaviour
    {
        public GameObject canvas;
        public GameObject window;
        public Button closeButton;
        public GameObject tabHeadersContainer;
        public GameObject tabBodiesContainer;
        public Button iconButton;

        public GameObject tabHeaderPrefab;
        public GameObject featureCodePopupPrefab;

        public GameObject outfitsTabPrefab;
        public GameObject localSettingsTabPrefab;
        public GameObject vanillaSettingsTabPrefab;
        public GameObject rolesSettingsTabPrefab;
    }
}
