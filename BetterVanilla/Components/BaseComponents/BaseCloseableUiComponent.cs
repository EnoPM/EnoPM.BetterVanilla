using System;
using BetterVanilla.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Components.BaseComponents;

public abstract class BaseCloseableUiComponent : MonoBehaviour
{
    public Canvas uiCanvas;
    public GameObject fullScreenOverlay;
    public GameObject basePanel;
    public GameObject panelHeader;
    public GameObject panelBody;

    public Image panelIcon;
    public TextMeshProUGUI titleText;
    public Button closeButton;

    private readonly PassiveButtonsBlocker _blocker = new();

    public bool IsOpened => uiCanvas.gameObject.activeSelf;

    protected virtual void Awake()
    {
        closeButton.onClick.AddListener(new Action(Close));
    }

    public virtual void Open()
    {
        _blocker.Block();
        uiCanvas.gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        _blocker.Unblock();
        uiCanvas.gameObject.SetActive(false);
    }
}