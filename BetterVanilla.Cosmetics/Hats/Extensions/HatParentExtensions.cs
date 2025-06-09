using PowerTools;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Hats.Extensions;

internal static class HatParentExtensions
{
    internal static void UpdateAssetMaterial(this HatParent hatParent, HatViewData asset)
    {
        if (asset.MatchPlayerColor)
        {
            hatParent.FrontLayer.sharedMaterial = CosmeticsContext.Hats.CachedMaterial;
            if (hatParent.BackLayer)
                hatParent.BackLayer.sharedMaterial = CosmeticsContext.Hats.CachedMaterial;
        }
        else
        {
            hatParent.FrontLayer.sharedMaterial = HatManager.Instance.DefaultShader;
            if (hatParent.BackLayer)
                hatParent.BackLayer.sharedMaterial = HatManager.Instance.DefaultShader;
        }
        var colorId = hatParent.matProperties.ColorId;
        PlayerMaterial.SetColors(colorId, hatParent.FrontLayer);
        if (hatParent.BackLayer)
        {
            PlayerMaterial.SetColors(colorId, hatParent.BackLayer);
        }
        hatParent.FrontLayer.material.SetInt(PlayerMaterial.MaskLayer, hatParent.matProperties.MaskLayer);
        if (hatParent.BackLayer)
            hatParent.BackLayer.material.SetInt(PlayerMaterial.MaskLayer, hatParent.matProperties.MaskLayer);
        var maskType = hatParent.matProperties.MaskType;
        switch (maskType)
        {
            case PlayerMaterial.MaskType.ScrollingUI:
            {
                if (hatParent.FrontLayer)
                    hatParent.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                if (hatParent.BackLayer)
                    hatParent.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                break;
            }
            case PlayerMaterial.MaskType.Exile:
            {
                if (hatParent.FrontLayer)
                    hatParent.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                if (hatParent.BackLayer)
                    hatParent.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                break;
            }
            case PlayerMaterial.MaskType.None:
            case PlayerMaterial.MaskType.SimpleUI:
            case PlayerMaterial.MaskType.ComplexUI:
            default:
            {
                if (hatParent.FrontLayer)
                    hatParent.FrontLayer.maskInteraction = SpriteMaskInteraction.None;
                if (hatParent.BackLayer)
                    hatParent.BackLayer.maskInteraction = SpriteMaskInteraction.None;
                break;
            }
        }
        if (hatParent.matProperties.MaskLayer > 0) return;
        PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(hatParent.FrontLayer, hatParent.matProperties.IsLocalPlayer);
        if (hatParent.BackLayer)
        {
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(hatParent.BackLayer, hatParent.matProperties.IsLocalPlayer);
        }
    }

    internal static void LateUpdateAsset(this HatParent hatParent, HatViewData asset)
    {
        if (!hatParent.Hat || !(asset != null)) return;
        if (hatParent.FrontLayer.sprite != asset.ClimbImage && hatParent.FrontLayer.sprite != asset.FloorImage)
        {
            if ((hatParent.Hat.InFront || asset.BackImage) && asset.LeftMainImage)
                hatParent.FrontLayer.sprite = hatParent.Parent.flipX || hatParent.shouldFaceLeft ? asset.LeftMainImage : asset.MainImage;
            if (asset.BackImage && asset.LeftBackImage)
            {
                hatParent.BackLayer.sprite = hatParent.Parent.flipX || hatParent.shouldFaceLeft ? asset.LeftBackImage : asset.BackImage;
            }
            else
            {
                if (asset.BackImage || hatParent.Hat.InFront || !asset.LeftMainImage)
                    return;
                hatParent.BackLayer.sprite = hatParent.Parent.flipX || hatParent.shouldFaceLeft ? asset.LeftMainImage : asset.MainImage;
            }
        }
        else
        {
            if (!(hatParent.FrontLayer.sprite == asset.ClimbImage) && !(hatParent.FrontLayer.sprite == asset.LeftClimbImage))
                return;
            var spriteAnimNodeSync = hatParent.SpriteSyncNode ?? hatParent.GetComponent<SpriteAnimNodeSync>();
            if (!spriteAnimNodeSync)
                return;
            spriteAnimNodeSync.NodeId = 0;
        }
    }

    internal static void PopulateFromAsset(this HatParent hatParent, HatViewData asset)
    {
        hatParent.UpdateMaterial();

        if (asset == null)
        {
            return;
        }

        var spriteAnimNodeSync = hatParent.SpriteSyncNode ?? hatParent.GetComponent<SpriteAnimNodeSync>();
        if (spriteAnimNodeSync)
        {
            spriteAnimNodeSync.NodeId = hatParent.Hat.NoBounce ? 1 : 0;
        }

        if (hatParent.Hat.InFront)
        {
            hatParent.BackLayer.enabled = false;
            hatParent.FrontLayer.enabled = true;
            hatParent.FrontLayer.sprite = asset.MainImage;
        }
        else if (asset.BackImage)
        {
            hatParent.BackLayer.enabled = true;
            hatParent.FrontLayer.enabled = true;
            hatParent.BackLayer.sprite = asset.BackImage;
            hatParent.FrontLayer.sprite = asset.MainImage;
        }
        else
        {
            hatParent.BackLayer.enabled = true;
            hatParent.FrontLayer.enabled = false;
            hatParent.FrontLayer.sprite = null;
            hatParent.BackLayer.sprite = asset.MainImage;
        }

        if (!hatParent.options.Initialized || !hatParent.HideHat()) return;
        hatParent.FrontLayer.enabled = false;
        hatParent.BackLayer.enabled = false;
    }
}