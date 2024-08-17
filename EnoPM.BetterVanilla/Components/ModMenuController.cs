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
    public Button closeButton;
    public GameObject tabHeadersContainer;
    public GameObject tabBodiesContainer;
    public GameObject outfitsTabPrefab;
    public GameObject settingsTabPrefab;

    internal DressingOutfitTabController DressingOutfitTab;
    internal SettingsTabController SettingsTab;

    private readonly PassiveButtonsBlocker _overlayBlocker = new();

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener((UnityAction)Close);

        DressingOutfitTab = Instantiate(outfitsTabPrefab, tabBodiesContainer.transform).GetComponent<DressingOutfitTabController>();
        Plugin.Logger.LogMessage($"Tab: {DressingOutfitTab.name}");
        SettingsTab = Instantiate(settingsTabPrefab, tabBodiesContainer.transform).GetComponent<SettingsTabController>();
        Plugin.Logger.LogMessage($"Tab: {SettingsTab.name}");
        canvas.SetActive(false);
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
}