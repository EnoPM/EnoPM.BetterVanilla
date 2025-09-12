using System;
using AmongUs.Data;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Cosmetics.Core.Utils;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BetterVanilla.Cosmetics.Hats.Patches;

[HarmonyPatch(typeof(HatsTab))]
internal static class HatsTabPatches
{
    
    [HarmonyPostfix, HarmonyPatch(nameof(HatsTab.SelectHat))]
    [HarmonyPatch(nameof(HatsTab.ClickEquip))]
    private static void SelectHatPostfix(VisorsTab __instance)
    {
        __instance.PlayerPreview.SetLocalVisorColor();
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatsTab.OnEnable))]
    private static bool OnEnablePrefix(HatsTab __instance)
    {
        __instance.OnTabEnable();
        return false;
    }

    private static void OnTabEnable(this HatsTab hatsTab)
    {
        hatsTab.InventoryTabOnEnable();
        var unlockedHats = HatManager.Instance.GetUnlockedHats();
        hatsTab.currentHat = HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat);

        var num = 0;
        for (var i = 0; i < unlockedHats.Length; ++i)
        {
            var hat = unlockedHats[i];
            var x = hatsTab.XRange.Lerp(num % hatsTab.NumPerRow / (hatsTab.NumPerRow - 1f));
            // ReSharper disable once PossibleLossOfFraction
            var y = hatsTab.YStart - num / hatsTab.NumPerRow * 1f * hatsTab.YOffset;
            var chip = Object.Instantiate(hatsTab.ColorTabPrefab, hatsTab.scroller.Inner);
            chip.transform.localPosition = new Vector3(x, y, -1f);
            hatsTab.ConfigureChipButtons(chip, hat);
            chip.Inner.SetMaskType(PlayerMaterial.MaskType.SimpleUI);
            hatsTab.UpdateMaterials(chip.Inner.FrontLayer, hat);
            var playerColor = hatsTab.HasLocalPlayer() ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId : DataManager.Player.Customization.Color;
            if (CosmeticsManager.Hats.TryGetCosmetic(hat.ProductId, out var asset))
            {
                chip.Inner.FrontLayer.sprite = asset.PreviewResource;
                PlayerMaterial.SetColors(playerColor, chip.Inner.FrontLayer);
            }
            else
            {
                hat.SetPreview(chip.Inner.FrontLayer, playerColor);
            }

            chip.Tag = hat;
            chip.SelectionHighlight.gameObject.SetActive(false);
            hatsTab.ColorChips.Add(chip);
            num++;
            if (!HatManager.Instance.CheckLongModeValidCosmetic(hat.ProdId, hatsTab.PlayerPreview.GetIgnoreLongMode()))
            {
                chip.SetUnavailable();
            }
        }
        hatsTab.currentHatIsEquipped = true;
        hatsTab.SetScrollerBounds();
    }

    private static void ConfigureChipButtons(this HatsTab hatsTab, ColorChip chip, HatData hatData)
    {
        if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
        {
            chip.Button.OnMouseOver.AddListener(new Action(() => hatsTab.SelectHat(hatData)));
            chip.Button.OnMouseOut.AddListener(new Action(() => hatsTab.SelectHat(HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat))));
            chip.Button.OnClick.AddListener(new Action(hatsTab.ClickEquip));
        }
        else
        {
            chip.Button.OnClick.AddListener(new Action(() => hatsTab.SelectHat(hatData)));
        }
        chip.Button.ClickMask = hatsTab.scroller.Hitbox;
    }
}