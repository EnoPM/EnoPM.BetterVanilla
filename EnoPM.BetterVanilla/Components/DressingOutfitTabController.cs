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
    public TMP_InputField outfitNameField;
    public Button saveOutfitButton;
    public GameObject outfitsContainerContent;
    public GameObject savedOutfitItemPrefab;

    internal readonly List<SavedOutfitController> SavedOutfits = [];

    protected override void Awake()
    {
        base.Awake();
        saveOutfitButton.onClick.AddListener((UnityAction)OnSaveOutfitButtonClick);
        outfitNameField.onValueChanged.AddListener((UnityAction<string>)OnOutfitNameFieldChange);
        saveOutfitButton.interactable = false;
        
        RefreshSavedOutfits();
    }

    private void OnOutfitNameFieldChange(string value)
    {
        saveOutfitButton.interactable = !string.IsNullOrWhiteSpace(value) && !DB.Outfits.Outfits.Any(x => string.Equals(x.Name, value, StringComparison.InvariantCultureIgnoreCase));
    }

    private void OnSaveOutfitButtonClick()
    {
        if (string.IsNullOrWhiteSpace(outfitNameField.text)) return;
        var outfit = DB.SaveCurrentOutfit(outfitNameField.text);
        RegisterOutfit(outfit);
        outfitNameField.text = string.Empty;
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