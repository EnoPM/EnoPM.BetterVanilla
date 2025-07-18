﻿using BetterVanilla.Components;
using UnityEngine;
using UnityEngine.UI;

namespace BetterVanilla.BetterModMenu;

public sealed class BetterModMenuButtonUi : MonoBehaviour
{
    public Button button = null!;

    public void OnButtonClicked()
    {
        if (BetterVanillaManager.Instance == null) return;
        BetterVanillaManager.Instance.BetterMenu.Show();
    }
}