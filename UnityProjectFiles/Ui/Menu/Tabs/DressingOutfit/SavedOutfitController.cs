using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components
{
    public class SavedOutfitController : MonoBehaviour
    {
        public TextMeshProUGUI outfitNameText;
        public Button button;
        public Button deleteButton;

        private void Awake()
        {
            deleteButton.onClick.AddListener((UnityAction)OnDeleteButtonClick);
        }

        private void OnDeleteButtonClick()
        {
            Destroy(gameObject);
        }
    }
}
