using UnityEngine;

namespace EnoPM.BetterVanilla.Components
{
    public class SettingsTabController : TabController
    {
        public GameObject settingsContainer;
        public string categoryId;

        public GameObject toggleSettingPrefab;
        public GameObject dropdownSettingPrefab;
        public GameObject sliderSettingPrefab;
    }
}