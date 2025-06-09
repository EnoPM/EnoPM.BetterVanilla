using System;
using System.Collections.Generic;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using BetterVanilla.Cosmetics.Api.Visors;
using BetterVanilla.Cosmetics.Core;
using BetterVanilla.Cosmetics.Core.Spritesheet;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BetterVanilla.Cosmetics.Visors;

public sealed class VisorCosmetic : BaseCosmetic<VisorViewData, VisorLayer, VisorData>, IVisor<Sprite>, IVisorExtension
{
    protected override Dictionary<string, VisorViewData> ViewDataCache => CosmeticsContext.Visors.ViewDataCache;
    public override string ProductId => "visor_bv_" + Name.Replace(' ', '_');
    
    public Sprite MainResource { get; set; }
    public Sprite? LeftResource { get; set; }
    public Sprite? ClimbResource { get; set; }
    public Sprite? FloorResource { get; set; }
    public bool BehindHats { get; set; }

    public VisorCosmetic(SerializedVisor visor, SpritesheetCache cache) : this(visor, cache.GetSprite(visor.MainResource))
    {
        LeftResource = visor.LeftResource == null ? null : cache.GetSprite(visor.LeftResource);
        ClimbResource = visor.ClimbResource == null ? null : cache.GetSprite(visor.ClimbResource);
        FloorResource = visor.FloorResource == null ? null : cache.GetSprite(visor.FloorResource);
    }
    
    private VisorCosmetic(SerializedVisor visor, Sprite mainResource) : base(visor.Name, visor.Author)
    {
        Adaptive = visor.Adaptive;
        BehindHats = visor.BehindHats;
        MainResource = mainResource;
    }
    
    public override bool IsMyParent(VisorLayer parent)
    {
        return parent != null && parent.visorData != null && parent.visorData.ProductId == ProductId;
    }

    public override VisorViewData ToViewData(VisorViewData viewData)
    {
        base.ToViewData(viewData);
        
        viewData.IdleFrame = MainResource;
        if (FloorResource != null)
        {
            viewData.FloorFrame = FloorResource;
        }
        else
        {
            viewData.FloorFrame = viewData.IdleFrame;
        }
        if (LeftResource != null)
        {
            viewData.LeftIdleFrame = LeftResource;
        }
        if (ClimbResource != null)
        {
            viewData.ClimbFrame = ClimbResource;
        }
        viewData.MatchPlayerColor = Adaptive;

        return viewData;
    }

    public override VisorData ToCosmeticData()
    {
        var cosmeticData = base.ToCosmeticData();
        
        if (!ViewDataCache.TryGetValue(Name, out var viewData))
        {
            viewData = ToViewData();
        }

        cosmeticData.PreviewCrewmateColor = viewData.MatchPlayerColor;
        cosmeticData.displayOrder = 99;
        cosmeticData.ProductId = ProductId;
        cosmeticData.behindHats = BehindHats;
        cosmeticData.ChipOffset = new Vector2(0f, 0f); // TODO: check offset value
        cosmeticData.Free = true;

        cosmeticData.ViewDataRef = new AssetReference(viewData.Pointer);
        
        return cosmeticData;
    }
}