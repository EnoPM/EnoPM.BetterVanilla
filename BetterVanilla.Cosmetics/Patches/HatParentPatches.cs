using System;
using System.IO;
using System.Linq;
using BetterVanilla.Cosmetics.Utils;
using HarmonyLib;
using PowerTools;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Patches;

[HarmonyPatch(typeof(HatParent))]
internal static class HatParentPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetHat), typeof(int)), HarmonyPriority(Priority.High)]
    private static void SetHatPrefix(HatParent __instance)
    {
        SetCustomHat(__instance);
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetHat), typeof(HatData), typeof(int))]
    private static bool SetHatPrefix(HatParent __instance, HatData hat, int color)
    {
        if (SetCustomHat(__instance)) return true;
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(color);
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetHat), typeof(int))]
    private static bool SetHatPrefix(HatParent __instance, int color)
    {
        if (!__instance.IsCached()) return true;
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(color);
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.UpdateMaterial))]
    private static bool UpdateMaterialPrefix(HatParent __instance)
    {
        HatViewData? asset;
        try
        {
            _ = __instance.viewAsset.GetAsset();
            return true;
        }
        catch
        {
            if (!__instance.TryGetCached(out asset))
            {
                return false;
            }
        }
        if (asset.MatchPlayerColor)
        {
            __instance.FrontLayer.sharedMaterial = CosmeticsManager.CachedShader;
            if (__instance.BackLayer)
                __instance.BackLayer.sharedMaterial = CosmeticsManager.CachedShader;
        }
        else
        {
            __instance.FrontLayer.sharedMaterial = DestroyableSingleton<HatManager>.Instance.DefaultShader;
            if (__instance.BackLayer)
                __instance.BackLayer.sharedMaterial = DestroyableSingleton<HatManager>.Instance.DefaultShader;
        }
        int colorId = __instance.matProperties.ColorId;
        PlayerMaterial.SetColors(colorId, __instance.FrontLayer);
        if (__instance.BackLayer)
        {
            PlayerMaterial.SetColors(colorId, __instance.BackLayer);
        }
        __instance.FrontLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);
        if (__instance.BackLayer)
            __instance.BackLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);
        var maskType = __instance.matProperties.MaskType;
        if (maskType == PlayerMaterial.MaskType.ScrollingUI)
        {
            if (__instance.FrontLayer)
                __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            if (__instance.BackLayer)
                __instance.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        else if (maskType == PlayerMaterial.MaskType.Exile)
        {
            if (__instance.FrontLayer)
                __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            if (__instance.BackLayer)
                __instance.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
        else
        {
            if (__instance.FrontLayer)
                __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.None;
            if (__instance.BackLayer)
                __instance.BackLayer.maskInteraction = SpriteMaskInteraction.None;
        }
        if (__instance.matProperties.MaskLayer <= 0)
        {
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.FrontLayer, __instance.matProperties.IsLocalPlayer);
            if (__instance.BackLayer)
            {
                PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.BackLayer, __instance.matProperties.IsLocalPlayer);
            }
        }
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.LateUpdate))]
    private static bool LateUpdatePrefix(HatParent __instance)
    {
        if (!__instance.Parent || !__instance.Hat) return false;
        if (!__instance.TryGetCached(out var hatViewData)) return true;
        if (__instance.FrontLayer.sprite != hatViewData.ClimbImage &&
            __instance.FrontLayer.sprite != hatViewData.FloorImage)
        {
            if ((__instance.Hat.InFront || hatViewData.BackImage) && hatViewData.LeftMainImage)
            {
                __instance.FrontLayer.sprite =
                    __instance.Parent.flipX ? hatViewData.LeftMainImage : hatViewData.MainImage;
            }

            if (hatViewData.BackImage && hatViewData.LeftBackImage)
            {
                __instance.BackLayer.sprite =
                    __instance.Parent.flipX ? hatViewData.LeftBackImage : hatViewData.BackImage;
                return false;
            }

            if (!hatViewData.BackImage && !__instance.Hat.InFront && hatViewData.LeftMainImage)
            {
                __instance.BackLayer.sprite =
                    __instance.Parent.flipX ? hatViewData.LeftMainImage : hatViewData.MainImage;
                return false;
            }
        }
        else if (__instance.FrontLayer.sprite == hatViewData.ClimbImage ||
                 __instance.FrontLayer.sprite == hatViewData.LeftClimbImage)
        {
            var spriteAnimNodeSync = __instance.SpriteSyncNode != null
                ? __instance.SpriteSyncNode
                : __instance.GetComponent<SpriteAnimNodeSync>();
            if (spriteAnimNodeSync)
            {
                spriteAnimNodeSync.NodeId = 0;
            }
        }

        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetFloorAnim))]
    private static bool SetFloorAnimPrefix(HatParent __instance)
    {
        if (!__instance.TryGetCached(out var hatViewData)) return true;
        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = hatViewData.FloorImage;
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetIdleAnim))]
    private static bool SetIdleAnimPrefix(HatParent __instance, int colorId)
    {
        if (!__instance.Hat) return false;
        if (!__instance.IsCached()) return true;
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(colorId);
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetClimbAnim))]
    private static bool SetClimbAnimPrefix(HatParent __instance)
    {
        if (!__instance.TryGetCached(out var hatViewData)) return true;
        if (!__instance.options.ShowForClimb) return false;
        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = hatViewData.ClimbImage;
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.PopulateFromViewData))]
    private static bool PopulateFromHatViewDataPrefix(HatParent __instance)
    {
        if (!__instance.TryGetCached(out var asset)) return true;
        __instance.UpdateMaterial();

        var spriteAnimNodeSync = __instance.SpriteSyncNode
            ? __instance.SpriteSyncNode
            : __instance.GetComponent<SpriteAnimNodeSync>();
        if (spriteAnimNodeSync)
        {
            spriteAnimNodeSync.NodeId = __instance.Hat.NoBounce ? 1 : 0;
        }

        if (__instance.Hat.InFront)
        {
            __instance.BackLayer.enabled = false;
            __instance.FrontLayer.enabled = true;
            __instance.FrontLayer.sprite = asset.MainImage;
        }
        else if (asset.BackImage)
        {
            __instance.BackLayer.enabled = true;
            __instance.FrontLayer.enabled = true;
            __instance.BackLayer.sprite = asset.BackImage;
            __instance.FrontLayer.sprite = asset.MainImage;
        }
        else
        {
            __instance.BackLayer.enabled = true;
            __instance.FrontLayer.enabled = false;
            __instance.FrontLayer.sprite = null;
            __instance.BackLayer.sprite = asset.MainImage;
        }

        if (!__instance.options.Initialized || !__instance.HideHat()) return false;
        __instance.FrontLayer.enabled = false;
        __instance.BackLayer.enabled = false;
        return false;
    }

    private static bool SetCustomHat(HatParent hatParent)
    {
        if (!TutorialManager.InstanceExists) return true;
        var dirPath = Path.Combine(StorageUtility.HatsDirectory, "Test");
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
        var d = new DirectoryInfo(dirPath);
        var filePaths = d.GetFiles("*.png").Select(x => x.FullName).ToArray();
        var hats = CosmeticsManager.CreateHatDetailsFromFileNames(filePaths, true);
        if (hats.Count <= 0) return false;
        try
        {
            hatParent.Hat = CosmeticsManager.CreateHatBehaviour(hats[0], true);
        }
        catch (Exception err)
        {
            CosmeticsPlugin.Logging.LogWarning($"Unable to create test hat \n{err}");
            return true;
        }

        return false;
    }
}