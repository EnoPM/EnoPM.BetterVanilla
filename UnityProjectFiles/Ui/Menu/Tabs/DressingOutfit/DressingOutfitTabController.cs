using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components
{
    public class DressingOutfitTabController : TabController
    {
        public TMP_InputField outfitNameField;
        public Button saveOutfitButton;
        public GameObject outfitsContainerContent;
        public GameObject savedOutfitItemPrefab;

        private void Awake()
        {
            saveOutfitButton.onClick.AddListener((UnityAction)OnSaveOutfitButtonClick);
        }

        private void OnSaveOutfitButtonClick()
        {
            if (string.IsNullOrWhiteSpace(outfitNameField.text)) return;
            var outfitItem = Instantiate(savedOutfitItemPrefab, outfitsContainerContent.transform).GetComponent<SavedOutfitController>();
            outfitItem.outfitNameText.SetText(outfitNameField.text);
            outfitNameField.text = string.Empty;
        }
    }
}
