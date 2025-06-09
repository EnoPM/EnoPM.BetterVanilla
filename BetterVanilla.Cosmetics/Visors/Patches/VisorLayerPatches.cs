using System;
using HarmonyLib;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Visors.Patches;

[HarmonyPatch(typeof(VisorLayer))]
internal static class VisorLayerPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(VisorLayer.SetVisor), typeof(VisorData), typeof(int))]
    private static bool SetVisorPrefix(VisorLayer __instance, VisorData data, int color)
    {
        if (__instance == null || __instance.visorData == null)
        {
            return true;
        }
        if (!CosmeticsContext.Visors.TryGetViewData(__instance.visorData.ProductId, out var viewData)) return true;

        if (data == null || data != __instance.visorData)
        {
            __instance.Image.sprite = null;
        }
        __instance.visorData = data;
        __instance.SetMaterialColor(color);
        __instance.PopulateFromViewData();
        
        
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(VisorLayer.UpdateMaterial))]
    private static bool UpdateMaterialPrefix(VisorLayer __instance)
    {
        if (__instance == null || __instance.visorData == null)
        {
            return true;
        }
        if (!CosmeticsContext.Visors.TryGetViewData(__instance.visorData.ProductId, out var asset))
        {
            return true;
        }
        CosmeticsContext.Visors.UpdateMaterialFromViewAsset(__instance, asset);
        return false;
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(VisorLayer.SetFloorAnim))]
    private static bool SetFloorAnimPrefix(VisorLayer __instance)
    {
        if (__instance == null || __instance.visorData == null)
        {
            return true;
        }
        if (!CosmeticsContext.Visors.TryGetViewData(__instance.visorData.ProductId, out var asset)) return true;
        __instance.Image.sprite = asset.FloorFrame;
        return false;
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(VisorLayer.SetClimbAnim))]
    private static bool SetClimbAnimPrefix(VisorLayer __instance, PlayerBodyTypes bodyType)
    {
        if (__instance == null || __instance.visorData == null)
        {
            return true;
        }
        if (!CosmeticsContext.Visors.TryGetViewData(__instance.visorData.ProductId, out var asset)) return true;
        if (__instance.options.HideDuringClimb || bodyType == PlayerBodyTypes.Horse)
        {
            return false;
        }
        __instance.transform.localPosition = new Vector3(__instance.transform.localPosition.x, __instance.transform.localPosition.y, -0.01f);
        __instance.Image.sprite = asset.ClimbFrame;
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(VisorLayer.PopulateFromViewData))]
    private static bool PopulateFromViewDataPrefix(VisorLayer __instance)
    {
        if (__instance == null || __instance.visorData == null)
        {
            return true;
        }
        if (!CosmeticsContext.Visors.TryGetViewData(__instance.visorData.ProductId, out var asset))
        {
            return true;
        }
        CosmeticsContext.Visors.PopulateParentFromAsset(__instance, asset);
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(VisorLayer.SetFlipX))]
    private static bool SetFlipXPrefix(VisorLayer __instance, bool flipX)
    {
        if (__instance.visorData == null) return true;
        __instance.Image.flipX = flipX;
        if (!CosmeticsContext.Visors.TryGetViewData(__instance.visorData.ProductId, out var asset))
        {
            return true;
        }
        if (flipX && asset.LeftIdleFrame)
        {
            __instance.Image.sprite = asset.LeftIdleFrame;
        }
        else
        {
            __instance.Image.sprite = asset.IdleFrame;
        }

        return false;
    }
}