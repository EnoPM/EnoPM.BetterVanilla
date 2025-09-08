using HarmonyLib;

namespace BetterVanilla.Cosmetics.Hats.Patches;

[HarmonyPatch(typeof(HatParent))]
internal static class HatParentPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetHat), typeof(int))]
    private static bool SetHatPrefix(HatParent __instance, int color)
    {
        if (__instance == null || __instance.Hat == null)
        {
            return true;
        }
        if (!CosmeticsManager.Hats.TryGetViewData(__instance.Hat.ProductId, out _)) return true;
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(color);
        
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.UpdateMaterial))]
    private static bool UpdateMaterialPrefix(HatParent __instance)
    {
        if (__instance == null || __instance.Hat == null)
        {
            return true;
        }
        if (!CosmeticsManager.Hats.TryGetViewData(__instance.Hat.ProductId, out var asset))
        {
            return true;
        }
        CosmeticsManager.Hats.UpdateMaterialFromViewAsset(__instance, asset);
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.LateUpdate))]
    private static bool LateUpdatePrefix(HatParent __instance)
    {
        if (!__instance.Parent || !__instance.Hat) return false;
        if (!CosmeticsManager.Hats.TryGetViewData(__instance.Hat.ProductId, out var asset))
        {
            return true;
        }
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetFloorAnim))]
    private static bool SetFloorAnimPrefix(HatParent __instance)
    {
        if (__instance == null || __instance.Hat == null)
        {
            return true;
        }
        if (!CosmeticsManager.Hats.TryGetViewData(__instance.Hat.ProductId, out var asset)) return true;
        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = asset.FloorImage;
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetIdleAnim))]
    private static bool SetIdleAnimPrefix(HatParent __instance, int colorId)
    {
        if (__instance.Hat == null) return false;
        if (!CosmeticsManager.Hats.TryGetViewData(__instance.Hat.ProductId, out _)) return true;
        
        //__instance.PopulateFromViewData();
        //__instance.SetMaterialColor(colorId);
        
        __instance.SetHat(colorId);
        __instance.transform.SetLocalZ(0.0f);
        
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.SetClimbAnim))]
    private static bool SetClimbAnimPrefix(HatParent __instance)
    {
        if (__instance == null || __instance.Hat == null)
        {
            return true;
        }
        if (!CosmeticsManager.Hats.TryGetViewData(__instance.Hat.ProductId, out var asset)) return true;
        if (!__instance.options.ShowForClimb) return false;
        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = asset.ClimbImage;
        return false;
    }

    [HarmonyPrefix, HarmonyPatch(nameof(HatParent.PopulateFromViewData))]
    private static bool PopulateFromHatViewDataPrefix(HatParent __instance)
    {
        if (__instance == null || __instance.Hat == null)
        {
            return true;
        }
        if (!CosmeticsManager.Hats.TryGetViewData(__instance.Hat.ProductId, out var asset))
        {
            return true;
        }
        CosmeticsManager.Hats.PopulateParentFromAsset(__instance, asset);
        return false;
    }
}