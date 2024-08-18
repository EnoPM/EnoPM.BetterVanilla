using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EnoPM.BetterVanilla.Components;

public abstract class TabController : MonoBehaviour
{
    public static readonly List<TabController> AllTabs = [];
    public Sprite tabHeaderSprite;
    public string tabHeaderTitle;
    
    private TabHeaderController _header;

    protected virtual void Awake()
    {
        _header = ModMenuController.Instance.CreateTabHeader();
        _header.SetSprite(tabHeaderSprite);
        _header.SetTitleText(tabHeaderTitle);
        _header.button.onClick.AddListener((UnityAction)Show);
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
        _header.button.interactable = false;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _header.button.interactable = true;
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