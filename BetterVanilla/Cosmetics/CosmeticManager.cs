using BetterVanilla.Components;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Core.Helpers;
using UnityEngine;

namespace BetterVanilla.Cosmetics;

public sealed class CosmeticManager
{
    public readonly CosmeticsUi Ui;
    public readonly CosmeticsUiButton Button;

    public CosmeticManager()
    {
        var bundle = AssetBundleUtils.LoadFromExecutingAssembly("BetterVanilla.Assets.cosmetics.ui");

        Ui = Object.Instantiate(
            bundle.LoadComponent<CosmeticsUi>("Assets/Ui/CosmeticsUi.prefab"),
            BetterVanillaManager.Instance.transform
        );
        
        Button = Object.Instantiate(
            bundle.LoadComponent<CosmeticsUiButton>("Assets/Ui/CosmeticsUiButton.prefab"),
            BetterVanillaManager.Instance.transform
        );

        bundle.Unload(false);
    }

    public void OpenUi()
    {
        if (!Ui) return;
        Ui.Open();
    }

    public void CloseUi()
    {
        if (!Ui) return;
        Ui.Close();
    }
}