using System;
using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Core.Data;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Components.Menu.Outfits;

public sealed class OutfitsTab : MonoBehaviour
{
    public Button saveCurrentOutfitButton;
    public GameObject outfitsContainer;
    public GameObject savedOutfitItemPrefab;

    public readonly List<SavedOutfitItem> AllSavedOutfits = [];

    private void Awake()
    {
        saveCurrentOutfitButton.onClick.AddListener(new Action(OnSaveCurrentOutfitButtonClick));
    }

    private void Start()
    {
        foreach (var outfitData in BetterVanillaManager.Instance.Database.Data.Outfits)
        {
            CreateOutfitUi(outfitData);
        }
        RefreshSelectedOutfit();
    }

    public void RefreshSelectedOutfit()
    {
        foreach (var outfit in AllSavedOutfits)
        {
            if (!outfit.itemButton || outfit.OutfitData == null)
            {
                continue;
            }
            outfit.itemButton.interactable = !outfit.OutfitData.IsEquipped();
        }
    }

    private void Update()
    {
        saveCurrentOutfitButton.interactable = AllSavedOutfits.All(x => x.OutfitData == null || !x.OutfitData.IsEquipped());
    }

    private void OnEnable()
    {
        RefreshSelectedOutfit();
    }

    private void OnSaveCurrentOutfitButtonClick()
    {
        var player = PlayerControl.LocalPlayer;
        if (!player || !player.Data) return;
        var outfit = player.Data.DefaultOutfit;
        if (outfit == null) return;
        var outfitData = new LocalOutfitData
        {
            Hat = outfit.HatId,
            Skin = outfit.SkinId,
            Visor = outfit.VisorId,
            Pet = outfit.PetId,
            Nameplate = outfit.NamePlateId
        };
        BetterVanillaManager.Instance.Database.Data.Outfits.Add(outfitData);
        BetterVanillaManager.Instance.Database.Save();
        CreateOutfitUi(outfitData);
        RefreshSelectedOutfit();
    }

    private void CreateOutfitUi(LocalOutfitData outfitData)
    {
        var outfitItem = Instantiate(savedOutfitItemPrefab, outfitsContainer.transform).GetComponent<SavedOutfitItem>();
        outfitItem.Initialize(outfitData);
        AllSavedOutfits.Add(outfitItem);
    }
}