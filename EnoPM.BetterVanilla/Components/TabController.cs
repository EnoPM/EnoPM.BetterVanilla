using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.Components;

public abstract class TabController : MonoBehaviour
{
    public static readonly List<TabController> AllTabs = [];
    public Button tabHeaderPrefab;
    
    private Button _headerButton;

    protected virtual void Awake()
    {
        _headerButton = Instantiate(tabHeaderPrefab, ModMenuController.Instance.tabHeadersContainer.transform);
        _headerButton.onClick.AddListener((UnityAction)Show);
        gameObject.SetActive(false);
        AllTabs.Add(this);
    }
    
    protected virtual void OnDestroy()
    {
        AllTabs.Remove(this);
    }

    public void Show()
    {
        ModMenuController.Instance.CloseOpenedTab();
        gameObject.SetActive(true);
        _headerButton.interactable = false;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _headerButton.interactable = true;
    }

    public bool IsOpened()
    {
        return gameObject.activeSelf;
    }

    public virtual bool IsAllowed()
    {
        return true;
    }
}