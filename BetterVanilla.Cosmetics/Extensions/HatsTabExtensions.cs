using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using BetterVanilla.Cosmetics.Data;
using TMPro;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Extensions;

public static class HatsTabExtensions
{
    private static TMP_Text? TextTemplate { get; set; }

    public static void SetupCustomHats(this HatsTab hatsTab)
    {
        hatsTab.PlayerPreview.gameObject.SetActive(true);
        if (hatsTab.HasLocalPlayer())
        {
            hatsTab.PlayerPreview.UpdateFromLocalPlayer(PlayerMaterial.MaskType.None);
        }
        else
        {
            hatsTab.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);
        }
        for (var i = 0; i < hatsTab.scroller.Inner.childCount; i++)
        {
            UnityEngine.Object.Destroy(hatsTab.scroller.Inner.GetChild(i).gameObject);
        }
        hatsTab.ColorChips = new Il2CppSystem.Collections.Generic.List<ColorChip>();

        HatData[] unlockedHats = HatManager.Instance.GetUnlockedHats();
        Dictionary<string, List<(HatData, HatExtension?)>> packages = new();

        foreach (var hatBehaviour in unlockedHats)
        {
            var ext = hatBehaviour.GetHatExtension();

            var packageName = ext == null ? CosmeticsManager.InnerslothHatPackageName : ext.Package;

            if (!packages.TryGetValue(packageName, out var package))
            {
                packages[packageName] = package = [];
            }

            package.Add((hatBehaviour, ext));
        }

        var yOffset = hatsTab.YStart;
        TextTemplate = GameObject.Find("HatsGroup").transform
            .FindChild("Text").GetComponent<TMP_Text>();

        var orderedKeys = packages.Keys.OrderBy(x =>
            x switch
            {
                CosmeticsManager.InnerslothHatPackageName => 1000,
                CosmeticsManager.DeveloperHatPackageName => 0,
                _ => 500
            });
        foreach (var key in orderedKeys)
        {
            yOffset = hatsTab.CreatePackage(packages[key], key, yOffset);
        }

        hatsTab.scroller.ContentYBounds.max = -(yOffset + 4.1f);
    }
    public static void SetupCustomHats2(this HatsTab hatsTab)
    {
        for (var i = 0; i < hatsTab.scroller.Inner.childCount; i++)
        {
            UnityEngine.Object.Destroy(hatsTab.scroller.Inner.GetChild(i).gameObject);
        }

        hatsTab.ColorChips = new Il2CppSystem.Collections.Generic.List<ColorChip>();
        var unlockedHats = DestroyableSingleton<HatManager>.Instance.GetUnlockedHats();
        var packages = new Dictionary<string, List<(HatData, HatExtension?)>>();

        foreach (var hatBehaviour in unlockedHats)
        {
            var ext = hatBehaviour.GetHatExtension();
            if (ext != null)
            {
                if (!packages.ContainsKey(ext.Package))
                {
                    packages[ext.Package] = [];
                }
                packages[ext.Package].Add((hatBehaviour, ext));
            }
            else
            {
                if (!packages.ContainsKey(CosmeticsManager.InnerslothHatPackageName))
                {
                    packages[CosmeticsManager.InnerslothHatPackageName] = [];
                }
                packages[CosmeticsManager.InnerslothHatPackageName].Add((hatBehaviour, null));
            }
        }

        var yOffset = hatsTab.YStart;
        TextTemplate = GameObject.Find("HatsGroup").transform.FindChild("Text").GetComponent<TextMeshPro>();

        var orderedKeys = packages.Keys.OrderBy(x =>
            x switch
            {
                CosmeticsManager.InnerslothHatPackageName => 1000,
                CosmeticsManager.DeveloperHatPackageName => 0,
                _ => 500
            });
        foreach (var key in orderedKeys)
        {
            var value = packages[key];
            yOffset = hatsTab.CreatePackage(value, key, yOffset);
        }

        hatsTab.scroller.ContentYBounds.max = -(yOffset + 4.1f);
    }

    private static float CreatePackage(this HatsTab hatsTab,
        List<(HatData, HatExtension?)> hats, string packageName, float yStart)
    {
        var isDefaultPackage = CosmeticsManager.InnerslothHatPackageName == packageName;
        if (!isDefaultPackage)
        {
            hats = hats.OrderBy(x => x.Item1.name).ToList();
        }

        var offset = yStart;
        if (TextTemplate != null)
        {
            var title = UnityEngine.Object.Instantiate(TextTemplate, hatsTab.scroller.Inner);
            title.transform.localPosition = new Vector3(2.25f, yStart, -1f);
            title.transform.localScale = Vector3.one * 1.5f;
            title.fontSize *= 0.5f;
            title.enableAutoSizing = false;
            hatsTab.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => { title.SetText(packageName); })));
            offset -= 0.8f * hatsTab.YOffset;
        }

        for (var i = 0; i < hats.Count; i++)
        {
            var (hat, ext) = hats[i];
            var xPos = hatsTab.XRange.Lerp(i % hatsTab.NumPerRow / (hatsTab.NumPerRow - 1f));
            var yPos = offset - Mathf.RoundToInt(i / (float)hatsTab.NumPerRow) * (isDefaultPackage ? 1f : 1.5f) * hatsTab.YOffset;
            var colorChip = UnityEngine.Object.Instantiate(hatsTab.ColorTabPrefab, hatsTab.scroller.Inner);
            if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
            {
                colorChip.Button.OnMouseOver.AddListener((Action)(() => hatsTab.SelectHat(hat)));
                colorChip.Button.OnMouseOut.AddListener((Action)(() => hatsTab.SelectHat(DestroyableSingleton<HatManager>.Instance.GetHatById(DataManager.Player.Customization.Hat))));
                colorChip.Button.OnClick.AddListener((Action)hatsTab.ClickEquip);
            }
            else
            {
                colorChip.Button.OnClick.AddListener((Action)(() => hatsTab.SelectHat(hat)));
            }
            colorChip.Button.ClickMask = hatsTab.scroller.Hitbox;
            colorChip.Inner.SetMaskType(PlayerMaterial.MaskType.ScrollingUI);
            hatsTab.UpdateMaterials(colorChip.Inner.FrontLayer, hat);
            var background = colorChip.transform.FindChild("Background");
            var foreground = colorChip.transform.FindChild("ForeGround");

            if (ext != null)
            {
                if (background != null)
                {
                    background.localPosition = Vector3.down * 0.243f;
                    var localScaleCache = background.localScale;
                    background.localScale = new Vector3(localScaleCache.x, 0.8f, localScaleCache.y);
                }
                if (foreground != null)
                {
                    foreground.localPosition = Vector3.down * 0.243f;
                }

                if (TextTemplate != null)
                {
                    var description = UnityEngine.Object.Instantiate(TextTemplate, colorChip.transform);
                    description.transform.localPosition = new Vector3(0f, -0.65f, -1f);
                    description.alignment = TextAlignmentOptions.Center;
                    description.transform.localScale = Vector3.one * 0.65f;
                    hatsTab.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => { description.SetText($"{hat.name}\nby {ext.Author}"); })));
                }
            }

            colorChip.transform.localPosition = new Vector3(xPos, yPos, -1f);
            colorChip.Inner.SetHat(hat, hatsTab.HasLocalPlayer() ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId : DataManager.Player.Customization.Color);
            colorChip.Inner.transform.localPosition = hat.ChipOffset;
            colorChip.Tag = hat;
            colorChip.SelectionHighlight.gameObject.SetActive(false);
            hatsTab.ColorChips.Add(colorChip);
            CosmeticsManager.Cache(colorChip.Inner);
        }

        return offset - (hats.Count - 1) / (float)hatsTab.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * hatsTab.YOffset -
               1.75f;
    }
}