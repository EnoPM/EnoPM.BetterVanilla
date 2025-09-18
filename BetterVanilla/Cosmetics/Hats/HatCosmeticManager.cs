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
        var maskType = parent.matProperties.MaskType;

        if (asset.MatchPlayerColor)
        {
            if (maskType == PlayerMaterial.MaskType.ComplexUI || maskType == PlayerMaterial.MaskType.ScrollingUI)
            {
                parent.BackLayer.sharedMaterial = HatManager.Instance.MaskedPlayerMaterial;
                parent.FrontLayer.sharedMaterial = HatManager.Instance.MaskedPlayerMaterial;
            }
            else
            {
                parent.BackLayer.sharedMaterial = HatManager.Instance.PlayerMaterial;
                parent.FrontLayer.sharedMaterial = HatManager.Instance.PlayerMaterial;
            }
        } 
        else if (maskType == PlayerMaterial.MaskType.ComplexUI || maskType == PlayerMaterial.MaskType.ScrollingUI)
        {
            parent.BackLayer.sharedMaterial = HatManager.Instance.MaskedMaterial;
            parent.FrontLayer.sharedMaterial = HatManager.Instance.MaskedMaterial;
        }
        else
        {
            parent.BackLayer.sharedMaterial = HatManager.Instance.DefaultShader;
            parent.FrontLayer.sharedMaterial = HatManager.Instance.DefaultShader;
        }

        switch (maskType)
        {
            case PlayerMaterial.MaskType.SimpleUI:
                parent.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                parent.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                break;
            case PlayerMaterial.MaskType.Exile:
                parent.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                parent.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                break;
            default:
                parent.BackLayer.maskInteraction = SpriteMaskInteraction.None;
                parent.FrontLayer.maskInteraction = SpriteMaskInteraction.None;
                break;
        }
        
        parent.BackLayer.material.SetInt(PlayerMaterial.MaskLayer, parent.matProperties.MaskLayer);
        parent.FrontLayer.material.SetInt(PlayerMaterial.MaskLayer, parent.matProperties.MaskLayer);

        if (asset.MatchPlayerColor)
        {
            PlayerMaterial.SetColors(parent.matProperties.ColorId, parent.BackLayer);
            PlayerMaterial.SetColors(parent.matProperties.ColorId, parent.FrontLayer);
        }
        
        if (parent.matProperties.MaskLayer > 0) return;
        
        PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(parent.BackLayer, parent.matProperties.IsLocalPlayer);
        PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(parent.FrontLayer, parent.matProperties.IsLocalPlayer);
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