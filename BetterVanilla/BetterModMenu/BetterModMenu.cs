using BetterVanilla.Components;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Core.Helpers;
using UnityEngine;

namespace BetterVanilla.BetterModMenu;

public sealed class BetterModMenu
{
    private BetterModMenuUi Ui { get; }
    private BetterModMenuButtonUi ButtonUi { get; }

    public BetterModMenu()
    {
        var bundle = AssetBundleUtils.LoadFromExecutingAssembly("BetterVanilla.Assets.menu.ui");
        Ui = Object.Instantiate(
            bundle.LoadComponent<BetterModMenuUi>("Assets/Ui/BetterModMenu.prefab"),
            BetterVanillaManager.Instance.transform
        );
        
        ButtonUi = Object.Instantiate(
            bundle.LoadComponent<BetterModMenuButtonUi>("Assets/Ui/BetterModMenuButton.prefab"),
            BetterVanillaManager.Instance.transform
        );

        bundle.Unload(false);
    }

    public void Show()
    {
        if (Ui == null) return;
        Ui.Show();
    }

    public void Hide()
    {
        if (Ui == null) return;
        Ui.Hide();
    }
}