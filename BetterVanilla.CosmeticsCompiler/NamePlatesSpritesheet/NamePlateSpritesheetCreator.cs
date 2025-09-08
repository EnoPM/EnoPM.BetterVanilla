using BetterVanilla.Cosmetics.Api.NamePlates;
using BetterVanilla.CosmeticsCompiler.Core;

namespace BetterVanilla.CosmeticsCompiler.NamePlatesSpritesheet;

public sealed class NamePlateSpritesheetCreator : BaseSpritesheetCreator<CreateNameplateSpritesheetOptions, LoadableNamePlate, SerializedNamePlate>
{
    public NamePlateSpritesheetCreator(CreateNameplateSpritesheetOptions options) : base(options, new LoadableNamePlate(options))
    {
        
    }

    protected override SerializedNamePlate Serialize(LoadableNamePlate namePlate)
    {
        return new SerializedNamePlate
        {
            Name = namePlate.Name,
            Adaptive = namePlate.Adaptive,
            Author = namePlate.Author,
            MainResource = Serialize(namePlate.MainResource)
        };
    }

    protected override List<SpriteFile> GetAllSprites() => Cosmetic.AllSprites;
}