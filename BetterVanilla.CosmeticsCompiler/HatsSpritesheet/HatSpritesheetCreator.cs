using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using BetterVanilla.Cosmetics.Api.Hats;
using BetterVanilla.CosmeticsCompiler.Core;

namespace BetterVanilla.CosmeticsCompiler.HatsSpritesheet;

public sealed class HatSpritesheetCreator : BaseSpritesheetCreator<CreateHatSpritesheetOptions, LoadableHat, SerializedHat>
{
    public HatSpritesheetCreator(CreateHatSpritesheetOptions options) : base(options, new LoadableHat(options))
    {
    }
    
    protected override SerializedHat Serialize(LoadableHat hat)
    {
        return new SerializedHat
        {
            Name = hat.Name,
            Adaptive = hat.Adaptive,
            Author = hat.Author,
            Bounce = hat.Bounce,
            NoVisors = hat.NoVisors,
            MainResource = Serialize(hat.MainResource),
            PreviewResource = Serialize(hat.PreviewResource),
            FlipResource = hat.FlipResource == null ? null : Serialize(hat.FlipResource),
            BackResource = hat.BackResource == null ? null : Serialize(hat.BackResource),
            BackFlipResource = hat.BackFlipResource == null ? null : Serialize(hat.BackFlipResource),
            ClimbResource = hat.ClimbResource == null ? null : Serialize(hat.ClimbResource),
            FrontAnimationFrames = hat.FrontAnimationFrames?.Select(Serialize).ToList(),
            BackAnimationFrames = hat.BackAnimationFrames?.Select(Serialize).ToList()
        };
    }
    
    protected override string SerializeToJson(SerializedHat cosmetic)
    {
        return JsonSerializer.Serialize(cosmetic, CosmeticsJsonContext.Default.SerializedHat);
    }

    protected override List<SpriteFile> GetAllSprites() => Cosmetic.AllSprites;
}