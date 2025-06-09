using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Cosmetics.Core.Manager;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Visors;

public sealed class VisorCosmeticManager : BaseCosmeticManager<VisorCosmetic, VisorViewData, VisorLayer, VisorData>
{
    protected override bool CanBeCached(VisorLayer parent)
    {
        return parent != null && parent.visorData != null;
    }

    protected override void PopulateParent(VisorLayer parent)
    {
        parent.PopulateFromViewData();
    }

    protected override List<VisorData> GetVanillaCosmeticData()
    {
        return HatManager.Instance.allVisors.ToList();
    }

    protected override void OverrideVanillaCosmeticData(List<VisorData> allCosmeticData)
    {
        HatManager.Instance.allVisors = allCosmeticData.ToArray();
    }

    protected override VisorLayer? GetPlayerParent(PlayerControl player)
    {
        if (player == null || player.Data == null || player.cosmetics == null)
        {
            return null;
        }
        return player.cosmetics.visor;
    }

    public override void RefreshAnimationFrames(PlayerPhysics playerPhysics)
    {
        var parent = GetPlayerParent(playerPhysics.myPlayer);
        if (parent == null || parent.visorData == null) return;
        if (!TryGetCosmetic(parent.visorData.ProductId, out var cosmetic))
        {
            return;
        }
        cosmetic.RefreshAnimatedFrames(parent, playerPhysics.FlipX);
    }

    public override void UpdateMaterialFromViewAsset(VisorLayer parent, VisorViewData asset)
    {
        var maskType = parent.matProperties.MaskType;
        if (parent.visorData != null && asset.MatchPlayerColor)
        {
            if (maskType is PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI)
            {
                parent.Image.sharedMaterial = HatManager.Instance.MaskedPlayerMaterial;
            }
            else
            {
                parent.Image.sharedMaterial = HatManager.Instance.PlayerMaterial;
            }
        }
        else if (maskType is PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI)
        {
            parent.Image.sharedMaterial = HatManager.Instance.MaskedMaterial;
        }
        else
        {
            parent.Image.sharedMaterial = HatManager.Instance.DefaultShader;
        }

        switch (maskType)
        {
            case PlayerMaterial.MaskType.SimpleUI:
                parent.Image.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                break;
            case PlayerMaterial.MaskType.Exile:
                parent.Image.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                break;
            default:
                parent.Image.maskInteraction = SpriteMaskInteraction.None;
                break;
        }
        parent.Image.material.SetInt(PlayerMaterial.MaskLayer, parent.matProperties.MaskLayer);
        if (parent.visorData != null && asset.MatchPlayerColor)
        {
            PlayerMaterial.SetColors(parent.matProperties.ColorId, parent.Image);
        }
        if (parent.matProperties.MaskLayer > 0) return;
        PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(parent.Image, parent.matProperties.IsLocalPlayer);
    }

    public override void PopulateParentFromAsset(VisorLayer parent, VisorViewData asset)
    {
        parent.UpdateMaterial();
        parent.transform.SetLocalZ(parent.DesiredLocalZPosition);
        parent.SetFlipX(parent.Image.flipX);
    }
}