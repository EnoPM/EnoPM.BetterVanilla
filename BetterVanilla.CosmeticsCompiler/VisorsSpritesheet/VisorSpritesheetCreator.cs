using BetterVanilla.Cosmetics.Api.Visors;
using BetterVanilla.CosmeticsCompiler.Core;
namespace BetterVanilla.CosmeticsCompiler.VisorsSpritesheet;

public sealed class VisorSpritesheetCreator : BaseSpritesheetCreator<CreateVisorSpritesheetOptions, LoadableVisor, SerializedVisor>
{
    public VisorSpritesheetCreator(CreateVisorSpritesheetOptions options) : base(options, new LoadableVisor(options))
    {
    }
    
    protected override SerializedVisor Serialize(LoadableVisor visor)
    {
        return new SerializedVisor
        {
            Name = visor.Name,
            Adaptive = visor.Adaptive,
            Author = visor.Author,
            BehindHats = visor.BehindHats,
            MainResource = Serialize(visor.MainResource),
            PreviewResource = Serialize(visor.PreviewResource),
            LeftResource = visor.LeftResource == null ? null : Serialize(visor.LeftResource),
            ClimbResource = visor.ClimbResource == null ? null : Serialize(visor.ClimbResource),
            FloorResource = visor.FloorResource == null ? null : Serialize(visor.FloorResource),
            FrontAnimationFrames = visor.FrontAnimationFrames?.Select(Serialize).ToList(),
        };
    }
    
    protected override List<SpriteFile> GetAllSprites() => Cosmetic.AllSprites;
}