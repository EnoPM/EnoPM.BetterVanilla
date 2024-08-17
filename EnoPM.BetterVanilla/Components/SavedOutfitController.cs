using EnoPM.BetterVanilla.Data.Database;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public class SavedOutfitController : MonoBehaviour
{
    public TextMeshProUGUI outfitNameText;
    public Button button;
    public Button deleteButton;

    internal DressingOutfit Outfit { get; set; }

    private void Awake()
    {
        deleteButton.onClick.AddListener((UnityAction)OnDeleteButtonClick);
    }

    private void Start()
    {
        outfitNameText.SetText(Outfit.Name);
        button.onClick.AddListener((UnityAction)Outfit.ApplyOutfitToLocalPlayer);
    }

    private void OnDeleteButtonClick()
    {
        Outfit.Delete();
        ModMenuController.Instance.DressingOutfitTab.SavedOutfits.Remove(this);
        Destroy(gameObject);
    }
}