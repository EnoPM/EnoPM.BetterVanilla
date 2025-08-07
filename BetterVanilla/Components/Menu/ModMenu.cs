using System;
using BetterVanilla.Components.BaseComponents;
using BetterVanilla.Components.Menu.Outfits;
using BetterVanilla.Components.Menu.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Components.Menu;

public sealed class ModMenu : BaseCloseableUiComponent
{
    public GameObject tabsContainer;
    public GameObject homeTabPrefab;
    public GameObject outfitsTabPrefab;
    public GameObject settingsTabPrefab;
    public Button homeTabHeader;
    public Button settingsTabHeader;
    public Button outfitsTabHeader;
    public Button presetsTabHeader;
    public Color activeTabColor;
    public Color inactiveTabColor;
    
    public HomeTab HomeTab { get; private set; }
    public OutfitsTab OutfitsTab { get; private set; }
    private SettingsTab SettingsTab { get; set; }

    protected override void Awake()
    {
        base.Awake();
        homeTabHeader.onClick.AddListener(new Action(OnHomeTabClick));
        settingsTabHeader.onClick.AddListener(new Action(OnSettingsTabClick));
        outfitsTabHeader.onClick.AddListener(new Action(OnOutfitsTabClick));
        presetsTabHeader.onClick.AddListener(new Action(OnPresetsTabClick));
        Close();
    }

    private void Start()
    {
        SetTabActive(homeTabHeader);
    }

    private void SetTabActive(Button tabHeader)
    {
        ResetAllTabHeaders();
        DeactivateAllTabs();
        tabHeader.interactable = false;
        tabHeader.targetGraphic.color = activeTabColor;
        if (homeTabHeader == tabHeader)
        {
            if (HomeTab == null)
            {
                HomeTab = Instantiate(homeTabPrefab, tabsContainer.transform).GetComponent<HomeTab>();
            }
            else
            {
                HomeTab.gameObject.SetActive(true);
            }
        }
        else if (settingsTabHeader == tabHeader)
        {
            if (SettingsTab == null)
            {
                SettingsTab = Instantiate(settingsTabPrefab, tabsContainer.transform).GetComponent<SettingsTab>();
            }
            else
            {
                SettingsTab.gameObject.SetActive(true);
            }
        }
        else if (outfitsTabHeader == tabHeader)
        {
            if (OutfitsTab == null)
            {
                OutfitsTab = Instantiate(outfitsTabPrefab, tabsContainer.transform).GetComponent<OutfitsTab>();
            }
            else
            {
                OutfitsTab.gameObject.SetActive(true);
            }
        }
        else if (presetsTabHeader == tabHeader)
        {
            // TODO - Setup PresetsTab
        }
    }

    private void DeactivateAllTabs()
    {
        if (HomeTab)
        {
            HomeTab.gameObject.SetActive(false);
        }
        if (OutfitsTab)
        {
            OutfitsTab.gameObject.SetActive(false);
        }
        if (SettingsTab)
        {
            SettingsTab.gameObject.SetActive(false);
        }
    }

    private void ResetAllTabHeaders()
    {
        homeTabHeader.interactable = true;
        homeTabHeader.targetGraphic.color = inactiveTabColor;
        
        settingsTabHeader.interactable = true;
        settingsTabHeader.targetGraphic.color = inactiveTabColor;
        
        outfitsTabHeader.interactable = true;
        outfitsTabHeader.targetGraphic.color = inactiveTabColor;
        
        presetsTabHeader.interactable = false;
        presetsTabHeader.targetGraphic.color = inactiveTabColor;
    }

    private void OnHomeTabClick()
    {
        SetTabActive(homeTabHeader);
    }
    
    private void OnSettingsTabClick()
    {
        SetTabActive(settingsTabHeader);
    }
    
    private void OnOutfitsTabClick()
    {
        SetTabActive(outfitsTabHeader);
    }
    
    private void OnPresetsTabClick()
    {
        SetTabActive(presetsTabHeader);
    }
}