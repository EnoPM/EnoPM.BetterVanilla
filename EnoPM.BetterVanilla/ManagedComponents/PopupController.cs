using System;
using System.Collections;
using EnoPM.BetterVanilla.Core;
using Il2CppInterop.Runtime.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnoPM.BetterVanilla.ManagedComponents;

public partial class PopupController : MonoBehaviour
{
    // ReSharper disable InconsistentNaming
    [ManagedByEditor] private GameObject canvas;
    [ManagedByEditor] private TextMeshProUGUI titleText;
    [ManagedByEditor] private TextMeshProUGUI bodyText;
    [ManagedByEditor] private Button closeButton;
    [ManagedByEditor] private Button confirmButton;
    // ReSharper restore InconsistentNaming

    private void Start()
    {
        SetActive(false);
        closeButton.onClick.AddListener((UnityAction)Close);
        confirmButton.onClick.AddListener((UnityAction)Close);
    }

    public void SetActive(bool value) => canvas.SetActive(value);

    [HideFromIl2Cpp]
    public IEnumerator CoShow(string title, string body)
    {
        titleText.SetText(title);
        bodyText.SetText(body);

        SetActive(true);
        
        while (canvas.active)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    public void Close() => SetActive(false);
}