using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Cosmetics.Core.Manager;
using PowerTools;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Hats;

public sealed class HatCosmeticManager : BaseCosmeticManager<HatCosmetic, HatViewData, HatParent, HatData>
{
    protected override bool CanBeCached(HatParent parent)
    {
        return parent != null && parent.Hat != null;
    }

    protected override void PopulateParent(HatParent parent)
    {
        parent.PopulateFromViewData();
    }

    protected override List<HatData> GetVanillaCosmeticData()
    {
        return HatManager.Instance.allHats.ToList();
    }

    protected override void OverrideVanillaCosmeticData(List<HatData> allCosmeticData)
    {
        HatManager.Instance.allHats = allCosmeticData.ToArray();
    }

    protected override HatParent? GetPlayerParent(PlayerControl player)
    {
        if (player == null || player.Data == null || player.cosmetics == null)
        {
            return null;
        }
        return player.cosmetics.hat;
    }

    public override void RefreshAnimationFrames(PlayerPhysics playerPhysics)
    {
        var parent = GetPlayerParent(playerPhysics.myPlayer);
        if (parent == null || parent.Hat == null) return;
        if (!TryGetCosmetic(parent.Hat.ProductId, out var cosmetic))
        {
            return;
        }
        cosmetic.RefreshAnimatedFrames(parent, playerPhysics.FlipX);
    }

    public override void UpdateMaterialFromViewAsset(HatParent parent, HatViewData asset)
    {
        if (asset.MatchPlayerColor)
        {
            parent.FrontLayer.sharedMaterial = HatManager.Instance.PlayerMaterial;
            if (parent.BackLayer)
            {
                parent.BackLayer.sharedMaterial = HatManager.Instance.PlayerMaterial;
            }
        }
        else
        {
            parent.FrontLayer.sharedMaterial = HatManager.Instance.DefaultShader;
            if (parent.BackLayer)
            {
                parent.BackLayer.sharedMaterial = HatManager.Instance.DefaultShader;
            }
        }
        var colorId = parent.matProperties.ColorId;
        PlayerMaterial.SetColors(colorId, parent.FrontLayer);
        if (parent.BackLayer)
        {
            PlayerMaterial.SetColors(colorId, parent.BackLayer);
        }
        parent.FrontLayer.material.SetInt(PlayerMaterial.MaskLayer, parent.matProperties.MaskLayer);
        if (parent.BackLayer)
        {
            parent.BackLayer.material.SetInt(PlayerMaterial.MaskLayer, parent.matProperties.MaskLayer);
        }
        var maskType = parent.matProperties.MaskType;
        switch (maskType)
        {
            case PlayerMaterial.MaskType.ScrollingUI:
            {
                if (parent.FrontLayer)
                    parent.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                if (parent.BackLayer)
                    parent.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                break;
            }
            case PlayerMaterial.MaskType.Exile:
            {
                if (parent.FrontLayer)
                    parent.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                if (parent.BackLayer)
                    parent.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                break;
            }
            case PlayerMaterial.MaskType.None:
            case PlayerMaterial.MaskType.SimpleUI:
            case PlayerMaterial.MaskType.ComplexUI:
            default:
            {
                if (parent.FrontLayer)
                {
                    parent.FrontLayer.maskInteraction = SpriteMaskInteraction.None;
                }

                if (parent.BackLayer)
                {
                    parent.BackLayer.maskInteraction = SpriteMaskInteraction.None;
                }
                break;
            }
        }
        if (parent.matProperties.MaskLayer > 0) return;
        PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(parent.FrontLayer, parent.matProperties.IsLocalPlayer);
        if (parent.BackLayer)
        {
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(parent.BackLayer, parent.matProperties.IsLocalPlayer);
        }
    }
    
    public override void PopulateParentFromAsset(HatParent parent, HatViewData asset)
    {
        parent.UpdateMaterial();

        if (asset == null)
        {
            return;
        }

        var spriteAnimNodeSync = parent.SpriteSyncNode ?? parent.GetComponent<SpriteAnimNodeSync>();
        if (spriteAnimNodeSync)
        {
            spriteAnimNodeSync.NodeId = parent.Hat.NoBounce ? 1 : 0;
        }

        if (parent.Hat.InFront)
        {
            parent.BackLayer.enabled = false;
            parent.FrontLayer.enabled = true;
            parent.FrontLayer.sprite = asset.MainImage;
        }
        else if (asset.BackImage)
        {
            parent.BackLayer.enabled = true;
            parent.FrontLayer.enabled = true;
            parent.BackLayer.sprite = asset.BackImage;
            parent.FrontLayer.sprite = asset.MainImage;
        }
        else
        {
            parent.BackLayer.enabled = true;
            parent.FrontLayer.enabled = false;
            parent.FrontLayer.sprite = null;
            parent.BackLayer.sprite = asset.MainImage;
        }

        if (!parent.options.Initialized || !parent.HideHat()) return;
        parent.FrontLayer.enabled = false;
        parent.BackLayer.enabled = false;
    }
}