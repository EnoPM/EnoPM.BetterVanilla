using System.Collections.Generic;
using BetterVanilla.Cosmetics.Api.NamePlates;
using BetterVanilla.Cosmetics.Core;
using BetterVanilla.Cosmetics.Core.Spritesheet;
using UnityEngine;

namespace BetterVanilla.Cosmetics.NamePlates;

public sealed class NamePlateCosmetic : BaseCosmetic<NamePlateViewData, PlayerVoteArea, NamePlateData>,
    INamePlate<Sprite>, INamePlateExtension
{
    public override string ProductId => "nameplate_bv_" + Name.Replace(' ', '_');

    protected override Dictionary<string, NamePlateViewData> ViewDataCache { get; }


    public Sprite MainResource { get; set; }

    public NamePlateCosmetic(SerializedNamePlate namePlate, SpritesheetCache cache) : this(namePlate,
        cache.GetSprite(namePlate.MainResource))
    {
    }

    private NamePlateCosmetic(SerializedNamePlate namePlate, Sprite mainResource) : base(namePlate.Name,
        namePlate.Author)
    {
        Adaptive = namePlate.Adaptive;
        MainResource = mainResource;
    }

    public override bool IsMyParent(PlayerVoteArea parent)
    {
        return parent != null && parent.Background.sprite == MainResource;
    }
}