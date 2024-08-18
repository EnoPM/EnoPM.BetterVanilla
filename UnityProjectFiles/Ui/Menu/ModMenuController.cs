using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components
{
    public class ModMenuController : MonoBehaviour
    {
        public GameObject canvas;
        public Button closeButton;
        public GameObject tabHeadersContainer;
        public GameObject tabBodiesContainer;

        public GameObject tabHeaderPrefab;

        public GameObject outfitsTabPrefab;
        public GameObject localSettingsTabPrefab;
        public GameObject vanillaSettingsTabPrefab;
        public GameObject rolesSettingsTabPrefab;
    }
}
