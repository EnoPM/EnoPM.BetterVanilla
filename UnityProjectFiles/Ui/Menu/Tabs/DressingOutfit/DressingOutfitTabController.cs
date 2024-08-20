using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components
{
    public class DressingOutfitTabController : TabController
    {
        public Button saveOutfitButton;
        public GameObject outfitsContainerContent;
        public GameObject savedOutfitItemPrefab;

        private void Awake()
        {
            saveOutfitButton.onClick.AddListener((UnityAction)OnSaveOutfitButtonClick);
        }

        private void OnSaveOutfitButtonClick()
        {
            var outfitItem = Instantiate(savedOutfitItemPrefab, outfitsContainerContent.transform).GetComponent<SavedOutfitController>();
        }
    }
}
