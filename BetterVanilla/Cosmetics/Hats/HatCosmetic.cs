using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using BetterVanilla.Core;
using BetterVanilla.Cosmetics.Api.Hats;
using BetterVanilla.Cosmetics.Core;
using BetterVanilla.Cosmetics.Core.Spritesheet;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BetterVanilla.Cosmetics.Hats;

public sealed class HatCosmetic : BaseCosmetic<HatViewData, HatParent, HatData>, IHat<Sprite>, IHatExtension
{
    protected override Dictionary<string, HatViewData> ViewDataCache => CosmeticsManager.Hats.ViewDataCache;

    public override string ProductId => "bv_hat_" + Name.Replace(' ', '_');

    public bool Bounce { get; set; }
    public bool NoVisors { get; set; }
    public Sprite MainResource { get; set; }
    public Sprite PreviewResource { get; set; }
    public Sprite? FlipResource { get; set; }
    public Sprite? BackResource { get; set; }
    public Sprite? BackFlipResource { get; set; }
    public Sprite? ClimbResource { get; set; }
    public List<Sprite>? FrontAnimationFrames { get; set; }
    public List<Sprite>? BackAnimationFrames { get; set; }

    public bool Behind { get; set; }

    public int CurrentFrontFrame { get; set; }

    public int CurrentBackFrame { get; set; }

    public int FrontDelay { get; set; } = 10;
    public int BackDelay { get; set; } = 10;

    public float FrontTime { get; set; }
    public float BackTime { get; set; }

    public HatCosmetic(SerializedHat hat, SpritesheetCache cache) : this(hat, cache.GetSprite(hat.MainResource))
    {
        PreviewResource = cache.GetSprite(hat.PreviewResource);
        FlipResource = hat.FlipResource != null ? cache.GetSprite(hat.FlipResource) : null;
        BackResource = hat.BackResource != null ? cache.GetSprite(hat.BackResource) : null;
        BackFlipResource = hat.BackFlipResource != null ? cache.GetSprite(hat.BackFlipResource) : null;
        ClimbResource = hat.ClimbResource != null ? cache.GetSprite(hat.ClimbResource) : null;
        FrontAnimationFrames = hat.FrontAnimationFrames?.Select(cache.GetSprite).ToList();
        BackAnimationFrames = hat.BackAnimationFrames?.Select(cache.GetSprite).ToList();
        NoVisors = hat.NoVisors;
    }

    private HatCosmetic(SerializedHat hat, Sprite mainResource) : base(hat.Name, hat.Author)
    {
        Adaptive = hat.Adaptive;
        Bounce = hat.Bounce;
        MainResource = mainResource;
    }

    public override HatViewData ToViewData(HatViewData viewData)
    {
        base.ToViewData(viewData);
        viewData.MainImage = viewData.FloorImage = MainResource;

        if (BackResource != null)
        {
            viewData.BackImage = BackResource;
            viewData.LeftBackImage = BackResource;
            Behind = true;
        }

        if (ClimbResource != null)
        {
            viewData.ClimbImage = viewData.LeftClimbImage = ClimbResource;
        }

        viewData.MatchPlayerColor = Adaptive;

        return viewData;
    }

    public override HatData ToCosmeticData()
    {
        var cosmeticData = base.ToCosmeticData();

        if (!ViewDataCache.TryGetValue(ProductId, out var viewData))
        {
            viewData = ToViewData();
        }

        cosmeticData.PreviewCrewmateColor = viewData.MatchPlayerColor;
        cosmeticData.displayOrder = 99;
        cosmeticData.ProductId = ProductId;
        cosmeticData.InFront = !Behind;
        cosmeticData.NoBounce = !Bounce;
        cosmeticData.BlocksVisors = NoVisors;
        cosmeticData.ChipOffset = new Vector2(0f, 0f); // TODO: check offset value

        cosmeticData.ViewDataRef = new AssetReference(viewData.Pointer);
        //cosmeticData.CreateAddressableAsset();

        return cosmeticData;
    }

    public override bool IsMyParent(HatParent parent)
    {
        return parent != null && parent.Hat != null && parent.Hat.ProductId == ProductId;
    }

    public override void AnimateFrames()
    {
        FrontTime += Time.deltaTime * 150;
        if (FrontTime >= FrontDelay)
        {
            CurrentFrontFrame = UpdateAnimationFrame(FrontAnimationFrames, CurrentFrontFrame);
            FrontTime = 0f;
        }
        BackTime += Time.deltaTime * 150;
        if (BackTime >= BackDelay)
        {
            CurrentBackFrame = UpdateAnimationFrame(BackAnimationFrames, CurrentBackFrame);
            BackTime = 0f;
        }
    }

    public override void RefreshAnimatedFrames(HatParent parent, bool flipX)
    {
        if (FrontAnimationFrames == null || FrontAnimationFrames.Count == 0)
        {
            if (FlipResource != null)
            {
                parent.FrontLayer.sprite = flipX ? FlipResource : MainResource;
            }
        }
        else
        {
            parent.FrontLayer.sprite = FrontAnimationFrames[CurrentFrontFrame];
        }

        if (BackAnimationFrames == null || BackAnimationFrames.Count == 0)
        {
            if (BackFlipResource != null)
            {
                parent.BackLayer.sprite = flipX ? BackFlipResource : BackResource ?? MainResource;
            }
        }
        else
        {
            parent.BackLayer.sprite = BackAnimationFrames[CurrentBackFrame];
        }
    }
}