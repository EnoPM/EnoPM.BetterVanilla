using System;
using System.IO;
using System.Linq;
using BetterVanilla.Cosmetics.Extensions;
using BetterVanilla.Cosmetics.Utils;
using HarmonyLib;
using PowerTools;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Patches;

[HarmonyPatch(typeof(HatParent))]
internal static class HatParentPatches
{
    

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetHat), typeof(int))]
    private static bool SetHatPrefix(HatParent __instance, int color)
    {
        if (!__instance.Hat.IsCached()) return true;
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(color);
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.UpdateMaterial))]
    private static bool UpdateMaterialPrefix(HatParent __instance)
    {
        if (!__instance.TryGetCached(out var asset))
        {
            return true;
        }
        __instance.UpdateAssetMaterial(asset);
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.LateUpdate))]
    private static bool LateUpdatePrefix(HatParent __instance)
    {
        if (!__instance.Parent || !__instance.Hat) return false;
        if (!__instance.TryGetCached(out var asset))
        {
            return true;
        }
        __instance.LateUpdateAsset(asset);
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
        if (!__instance.Hat.IsCached()) return true;
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(colorId);
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetClimbAnim))]
    private static bool SetClimbAnimPrefix(HatParent __instance)
    {
        if (!__instance.TryGetCached(out var asset)) return true;
        if (!__instance.options.ShowForClimb) return false;
        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = asset.ClimbImage;
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.PopulateFromViewData))]
    private static bool PopulateFromHatViewDataPrefix(HatParent __instance)
    {
        if (!__instance.TryGetCached(out var asset)) return true;
        __instance.PopulateFromAsset(asset);
        return false;
    }
}