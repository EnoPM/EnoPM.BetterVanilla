using System;
using AmongUs.Data;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Cosmetics.Core.Utils;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BetterVanilla.Cosmetics.Visors.Patches;

[HarmonyPatch(typeof(VisorsTab))]
internal static class VisorsTabPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(VisorsTab.OnEnable))]
    private static bool OnEnablePrefix(VisorsTab __instance)
    {
        __instance.OnTabEnable();
        return false;
    }

    [HarmonyPostfix, HarmonyPatch(nameof(VisorsTab.SelectVisor))]
    [HarmonyPatch(nameof(VisorsTab.ClickEquip))]
    private static void SelectVisorPostfix(VisorsTab __instance)
    {
        __instance.PlayerPreview.SetLocalVisorColor();
    }

    private static void OnTabEnable(this VisorsTab visorsTab)
    {
        visorsTab.InventoryTabOnEnable();
        var unlockedVisors = HatManager.Instance.GetUnlockedVisors();
        
        for (var i = 0; i < unlockedVisors.Count; ++i)
        {
            var visor = unlockedVisors[i];
            var x = visorsTab.XRange.Lerp(i % visorsTab.NumPerRow / (visorsTab.NumPerRow - 1f));
            // ReSharper disable once PossibleLossOfFraction
            var y = visorsTab.YStart - i / visorsTab.NumPerRow * visorsTab.YOffset;
            var chip = Object.Instantiate(visorsTab.ColorTabPrefab, visorsTab.scroller.Inner);
            chip.transform.localPosition = new Vector3(x, y, -1f);
            visorsTab.ConfigureChipButtons(chip, visor);
            chip.ProductId = visor.ProductId;
            visorsTab.UpdateMaterials(chip.Inner.FrontLayer, visor);
            var playerColor = visorsTab.HasLocalPlayer() ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId : DataManager.Player.Customization.Color;
            if (CosmeticsManager.Visors.TryGetCosmetic(visor.ProductId, out var asset))
            {
                chip.Inner.FrontLayer.sprite = asset.PreviewResource;
                PlayerMaterial.SetColors(playerColor, chip.Inner.FrontLayer);
            }
            else
            {
                visor.SetPreview(chip.Inner.FrontLayer, playerColor);
            }
            
            chip.Tag = visor.ProdId;
            chip.SelectionHighlight.gameObject.SetActive(false);
            visorsTab.ColorChips.Add(chip);
            if (!HatManager.Instance.CheckLongModeValidCosmetic(visor.ProdId, visorsTab.PlayerPreview.GetIgnoreLongMode()))
            {
                chip.SetUnavailable();
            }
        }
        if (unlockedVisors.Length != 0)
        {
            visorsTab.GetDefaultSelectable().PlayerEquippedForeground.SetActive(true);
        }
        visorsTab.visorId = DataManager.Player.Customization.Visor;
        visorsTab.currentVisorIsEquipped = true;
        visorsTab.SetScrollerBounds();
    }

    private static void ResetVisor(this VisorsTab visorsTab)
    {
        var visorId = DataManager.player.Customization.Visor;
        var visor = HatManager.Instance.GetVisorById(visorId);
        visorsTab.SelectVisor(visor);
        visorsTab.currentVisorIsEquipped = true;
    }
    
    private static void ConfigureChipButtons(this VisorsTab visorsTab, ColorChip chip, VisorData visorData)
    {
        if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
        {
            chip.Button.OnMouseOver.AddListener(new Action(() => visorsTab.SelectVisor(visorData)));
            chip.Button.OnMouseOut.AddListener(new Action(visorsTab.ResetVisor));
            chip.Button.OnClick.AddListener(new Action(visorsTab.ClickEquip));
        }
        else
        {
            chip.Button.OnClick.AddListener(new Action(() => visorsTab.SelectVisor(visorData)));
        }
        chip.Button.ClickMask = visorsTab.scroller.Hitbox;
    }
}