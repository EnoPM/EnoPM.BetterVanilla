using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
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
    
    protected override string SerializeToJson(SerializedNamePlate cosmetic)
    {
        return JsonSerializer.Serialize(cosmetic, CosmeticsJsonContext.Default.SerializedNamePlate);
    }

    protected override List<SpriteFile> GetAllSprites() => Cosmetic.AllSprites;
}