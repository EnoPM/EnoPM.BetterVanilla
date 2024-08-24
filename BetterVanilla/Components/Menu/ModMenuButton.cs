using System;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.Components.Menu;

public sealed class ModMenuButton : MonoBehaviour
{
    public Button button;

    private void Awake()
    {
        button.onClick.AddListener(new Action(OnClick));
    }

    private void Update()
    {
        button.interactable = BetterVanillaManager.Instance && BetterVanillaManager.Instance.Menu && HudManager.InstanceExists;
    }

    private void OnClick()
    {
        if (BetterVanillaManager.Instance.Menu.IsOpened)
        {
            BetterVanillaManager.Instance.Menu.Close();
        }
        else
        {
            BetterVanillaManager.Instance.Menu.Open();
        }
    }
}