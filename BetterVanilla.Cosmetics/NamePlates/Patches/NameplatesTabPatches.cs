using System;
using System.Linq;
using AmongUs.Data;
using BetterVanilla.Cosmetics.Core.Utils;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BetterVanilla.Cosmetics.NamePlates.Patches;

[HarmonyPatch(typeof(NameplatesTab))]
internal static class NameplatesTabPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(NameplatesTab.OnEnable))]
    private static bool OnEnablePrefix(NameplatesTab __instance)
    {
        return true;
        __instance.OnTabEnable();
        return false;
    }

    private static void OnTabEnable(this NameplatesTab namePlatesTab)
    {
        namePlatesTab.InventoryTabOnEnable();
        namePlatesTab.PlayerPreview.gameObject.SetActive(false);
        namePlatesTab.StartCoroutine(namePlatesTab.CoLoadNameplatePreview());

        var unlockedNamePlates = HatManager.Instance.GetUnlockedNamePlates().ToList();
        for (var i = 0; i < unlockedNamePlates.Count; ++i)
        {
            var namePlate = unlockedNamePlates[i];
            var x = namePlatesTab.XRange.Lerp(i % namePlatesTab.NumPerRow / (namePlatesTab.NumPerRow - 1f));
            // ReSharper disable once PossibleLossOfFraction
            var y = namePlatesTab.YStart - i / namePlatesTab.NumPerRow * namePlatesTab.YOffset;
            var chip = Object.Instantiate(namePlatesTab.ColorTabPrefab, namePlatesTab.scroller.Inner);
            chip.transform.localPosition = new Vector3(x, y, -1f);
            namePlatesTab.ConfigureChipButtons(chip, namePlate);
            chip.ProductId = namePlate.ProductId;
            var playerColor = namePlatesTab.HasLocalPlayer() ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId : DataManager.Player.Customization.Color;
            if (CosmeticsPlugin.Instance.NamePlates.TryGetViewData(namePlate.ProductId, out var asset))
            {
                
            }
            else
            {
                //namePlate.SetPreview(chip.Inner.FrontLayer, asset);
            }
            var namePlateChip = chip.Cast<NameplateChip>();
            if (namePlateChip == null)
            {
                throw new Exception("chip cannot be cast to NameplateChip");
            }
        }
    }
    
    private static void ResetNamePlate(this NameplatesTab namePlatesTab)
    {
        var namePlateId = DataManager.player.Customization.NamePlate;
        var namePlate = HatManager.Instance.GetNamePlateById(namePlateId);
        namePlatesTab.SelectNameplate(namePlate);
        namePlatesTab.currentNameplateIsEquipped = true;
    }
    
    private static void ConfigureChipButtons(this NameplatesTab namePlatesTab, ColorChip chip, NamePlateData namePlateData)
    {
        if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
        {
            chip.Button.OnMouseOver.AddListener(new Action(() => namePlatesTab.SelectNameplate(namePlateData)));
            chip.Button.OnMouseOut.AddListener(new Action(namePlatesTab.ResetNamePlate));
            chip.Button.OnClick.AddListener(new Action(namePlatesTab.ClickEquip));
        }
        else
        {
            chip.Button.OnClick.AddListener(new Action(() => namePlatesTab.SelectNameplate(namePlateData)));
        }
        chip.Button.ClickMask = namePlatesTab.scroller.Hitbox;
    }
}