using System;
using BetterVanilla.Components;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Cosmetics;

public class CosmeticsUiButton : MonoBehaviour
{
    public Button button;

    private void Awake()
    {
        //button.onClick.AddListener(new Action(OnButtonClicked));
        // TODO: Not disable
        //gameObject.SetActive(false);
    }

    public void OnButtonClicked()
    {
        BetterVanillaManager.Instance.Cosmetics.OpenUi();
    }
}