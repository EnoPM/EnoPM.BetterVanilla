using System.Linq;
using EnoPM.BetterVanilla.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public class ModMenuController : MonoBehaviour
{

    public static ModMenuController Instance { get; private set; }

    public GameObject canvas;
    public GameObject window;
    public Button closeButton;
    public GameObject tabHeadersContainer;
    public GameObject tabBodiesContainer;
    public Button iconButton;
    
    public GameObject tabHeaderPrefab;
    public GameObject featureCodePopupPrefab;
    
    public GameObject outfitsTabPrefab;
    public GameObject localSettingsTabPrefab;
    public GameObject vanillaSettingsTabPrefab;
    public GameObject rolesSettingsTabPrefab;
    
    private readonly PassiveButtonsBlocker _overlayBlocker = new();
    public SettingsTabController LocalSettingsTab { get; set; }
    public SettingsTabController VanillaSettingsTab { get; set; }
    //public SettingsTabController RolesSettingsTab { get; set; }
    public DressingOutfitTabController DressingOutfitTab { get; set; }
    

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener((UnityAction)Close);
        iconButton.onClick.AddListener((UnityAction)OnIconButtonClick);

        LocalSettingsTab = InstantiateTab<SettingsTabController>(localSettingsTabPrefab);
        VanillaSettingsTab = InstantiateTab<SettingsTabController>(vanillaSettingsTabPrefab);
        //RolesSettingsTab = InstantiateTab<SettingsTabController>(rolesSettingsTabPrefab);
        DressingOutfitTab = InstantiateTab<DressingOutfitTabController>(outfitsTabPrefab);
        
        canvas.SetActive(false);
    }

    private TTabController InstantiateTab<TTabController>(GameObject prefab) where TTabController : TabController
    {
        return Instantiate(prefab, tabBodiesContainer.transform).GetComponent<TTabController>();
    }

    private void OnIconButtonClick()
    {
        Instantiate(featureCodePopupPrefab, window.transform);
    }

    public void Open()
    {
        canvas.SetActive(true);
        _overlayBlocker.Block();
        
        if (TabController.AllTabs.Any(x => x.IsOpened())) return;
        var allowedTab = TabController.AllTabs.FirstOrDefault(x => x.IsAllowed());
        allowedTab?.Show();
    }

    public void Close()
    {
        canvas.SetActive(false);
        _overlayBlocker.Unblock();
    }

    public void CloseOpenedTab()
    {
        TabController.AllTabs.Find(x => x.IsOpened())?.Hide();
    }

    public TabHeaderController CreateTabHeader()
    {
        return Instantiate(tabHeaderPrefab, tabHeadersContainer.transform).GetComponent<TabHeaderController>();
    }
}