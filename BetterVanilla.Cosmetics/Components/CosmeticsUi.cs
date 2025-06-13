using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Cosmetics.Components;

public class CosmeticsUi : MonoBehaviour
{
    public GameObject canvas;
    public Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(new Action(OnCloseButtonClick));
    }

    private void OnCloseButtonClick()
    {
        
    }
}