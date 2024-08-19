using System.Linq;
using EnoPM.BetterVanilla.Core;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public class ModMenuController : MonoBehaviour
{

    public static ModMenuController Instance { get; private set; }

    public GameObject canvas;
    public Button closeButton;
    public GameObject tabHeadersContainer;
    public GameObject tabBodiesContainer;
    
    public GameObject tabHeaderPrefab;
    
    public GameObject outfitsTabPrefab;
    public GameObject localSettingsTabPrefab;
    public GameObject vanillaSettingsTabPrefab;
    public GameObject rolesSettingsTabPrefab;
    
    private readonly PassiveButtonsBlocker _overlayBlocker = new();
    
    public SettingsTabController LocalSettingsTab { get; set; }
    public SettingsTabController VanillaSettingsTab { get; set; }
    public SettingsTabController RolesSettingsTab { get; set; }
    public DressingOutfitTabController DressingOutfitTab { get; set; }
    

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener((UnityAction)Close);

        LocalSettingsTab = InstantiateTab<SettingsTabController>(localSettingsTabPrefab);
        VanillaSettingsTab = InstantiateTab<SettingsTabController>(vanillaSettingsTabPrefab);
        RolesSettingsTab = InstantiateTab<SettingsTabController>(rolesSettingsTabPrefab);
        DressingOutfitTab = InstantiateTab<DressingOutfitTabController>(outfitsTabPrefab);
        
        canvas.SetActive(false);
    }

    private TTabController InstantiateTab<TTabController>(GameObject prefab) where TTabController : TabController
    {
        return Instantiate(prefab, tabBodiesContainer.transform).GetComponent<TTabController>();
    }

    public void Open()
    {
        canvas.SetActive(true);
        _overlayBlocker.Block();
        if (PlayerControl.LocalPlayer)
        {
            PlayerControl.LocalPlayer.moveable = false;
            PlayerControl.LocalPlayer.NetTransform.Halt();
        }
        
        if (TabController.AllTabs.Any(x => x.IsOpened())) return;
        var allowedTab = TabController.AllTabs.FirstOrDefault(x => x.IsAllowed());
        allowedTab?.Show();
    }

    public void Close()
    {
        canvas.SetActive(false);
        _overlayBlocker.Unblock();
        if (PlayerControl.LocalPlayer)
        {
            PlayerControl.LocalPlayer.moveable = true;
            PlayerControl.LocalPlayer.NetTransform.Halt();
        }
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