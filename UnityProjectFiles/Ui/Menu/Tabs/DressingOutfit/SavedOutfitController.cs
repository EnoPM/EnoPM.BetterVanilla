using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components
{
    public class SavedOutfitController : MonoBehaviour
    {
        public Button button;
        public Button deleteButton;
        public Image hatPreview;
        public Image skinPreview;
        public Image visorPreview;
        public Image petPreview;
        public Image nameplatePreview;

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
