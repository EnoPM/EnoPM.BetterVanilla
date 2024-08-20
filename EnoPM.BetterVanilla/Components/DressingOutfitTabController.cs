using System;
using System.Collections.Generic;
using System.Linq;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Core.Data.Database;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public class DressingOutfitTabController : TabController
{
    public static DressingOutfitTabController Instance { get; private set; }
    
    public Button saveOutfitButton;
    public GameObject outfitsContainerContent;
    public GameObject savedOutfitItemPrefab;

    internal readonly List<SavedOutfitController> SavedOutfits = [];

    public void RefreshSelectedOutfit()
    {
        foreach (var outfit in SavedOutfits)
        {
            if (!outfit.button || outfit.Outfit == null) continue;
            outfit.button.interactable = !outfit.Outfit.IsEquipped();
        }
    }

    private void OnEnable()
    {
        RefreshSelectedOutfit();
    }

    private void Start()
    {
        RefreshSelectedOutfit();
    }

    protected override void Awake()
    {
        base.Awake();
        saveOutfitButton.onClick.AddListener((UnityAction)OnSaveOutfitButtonClick);
        
        RefreshSavedOutfits();
        Instance = this;
    }

    private void OnSaveOutfitButtonClick()
    {
        var outfit = DB.SaveCurrentOutfit();
        if (outfit == null) return;
        RegisterOutfit(outfit);
    }

    private void ClearSavedOutfits()
    {
        foreach (var outfitController in SavedOutfits)
        {
            Destroy(outfitController.gameObject);
        }
        
        SavedOutfits.Clear();
    }

    private void RefreshSavedOutfits()
    {
        ClearSavedOutfits();
        var outfits = DB.Outfits.Outfits;
        foreach (var outfit in outfits)
        {
            RegisterOutfit(outfit);
        }
    }

    private void RegisterOutfit(DressingOutfit outfit)
    {
        var outfitItem = Instantiate(savedOutfitItemPrefab, outfitsContainerContent.transform).GetComponent<SavedOutfitController>();
        outfitItem.Outfit = outfit;
        SavedOutfits.Add(outfitItem);
    }
}