using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using BetterVanilla.Cosmetics.Api.Hats;
using BetterVanilla.CosmeticsCompiler.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BetterVanilla.CosmeticsCompiler.HatsSpritesheet;

public sealed class HatSpritesheetCreator : IDisposable
{
    private CreateHatSpritesheetOptions Options { get; }
    private LoadableHat Hat { get; }
    private string SpritesheetOutputPath { get; }
    private string HatManifestOutputPath { get; }

    public HatSpritesheetCreator(CreateHatSpritesheetOptions options)
    {
        Options = options;
        Hat = new LoadableHat(Options);
        SpritesheetOutputPath = Path.Combine(Options.OutputDirectoryPath, Hat.Name + ".png");
        HatManifestOutputPath = Path.Combine(Options.OutputDirectoryPath, Hat.Name + ".spritesheet.json");
    }

    public void Process()
    {
        LoadAllSprites();
        
        CalculateSpritePositions();
        
        using var spritesheet = DrawSpritesheet();
        spritesheet.Save(SpritesheetOutputPath);
        
        var manifest = Serialize(Hat);
        var serializedManifest = JsonSerializer.Serialize(manifest);
        File.WriteAllText(HatManifestOutputPath, serializedManifest);
    }

    private Image<Rgba32> DrawSpritesheet()
    {
        var width = Hat.AllSprites.Max(x => x.Sprite.X + x.Sprite.Width);
        var height = Hat.AllSprites.Max(x => x.Sprite.Y + x.Sprite.Height);

        var spriteSheet = new Image<Rgba32>(width, height);
        foreach (var spriteFile in Hat.AllSprites)
        {
            var sprite = spriteFile.Sprite;
            var image = sprite.Image;
            if (image == null)
            {
                throw new Exception($"Sprite not loaded: {spriteFile.Sprite.Name} - {spriteFile.Path}");
            }
            spriteSheet.Mutate(
                ctx => ctx.DrawImage(
                    image,
                    new Point(sprite.X, sprite.Y),
                    1f
                )
            );
        }
        return spriteSheet;
    }

    private SerializedHat Serialize(LoadableHat hat)
    {
        return new SerializedHat
        {
            Name = hat.Name,
            Adaptive = hat.Adaptive,
            Author = hat.Author,
            Bounce = hat.Bounce,
            NoVisors = hat.NoVisors,
            MainResource = Serialize(hat.MainResource),
            FlipResource = hat.FlipResource == null ? null : Serialize(hat.FlipResource),
            BackResource = hat.BackResource == null ? null : Serialize(hat.BackResource),
            BackFlipResource = hat.BackFlipResource == null ? null : Serialize(hat.BackFlipResource),
            ClimbResource = hat.ClimbResource == null ? null : Serialize(hat.ClimbResource),
            FrontAnimationFrames = hat.FrontAnimationFrames?.Select(Serialize).ToList(),
            BackAnimationFrames = hat.BackAnimationFrames?.Select(Serialize).ToList()
        };
    }

    private SerializedSprite Serialize(SpriteFile spriteFile)
    {
        return new SerializedSprite
        {
            Path = SpritesheetOutputPath,
            X = spriteFile.Sprite.X,
            Y = spriteFile.Sprite.Y,
            Width = spriteFile.Sprite.Width,
            Height = spriteFile.Sprite.Height,
        };
    }

    private void CalculateSpritePositions(int padding = 2)
    {
        var maxWidth = CalculateMaxSpritesheetWidth();

        var x = 0;
        var y = 0;
        var height = 0;

        var spriteFiles = GetSortedSpriteFiles();
        foreach (var file in spriteFiles)
        {
            var image = file.Sprite.Image;
            if (image == null)
            {
                throw new Exception($"Sprite not loaded: {file.Sprite.Name} - {file.Path}");
            }
            if (x + image.Width > maxWidth)
            {
                x = 0;
                y += height + padding;
                height = 0;
            }
            file.Sprite.X = x;
            file.Sprite.Y = y;

            x += image.Width + padding;
            height = Math.Max(height, image.Height);
        }
    }

    private List<SpriteFile> GetSortedSpriteFiles()
    {
        return Hat.AllSprites
            .OrderByDescending(x => x.Sprite.Height)
            .ToList();
    }

    private int CalculateMaxSpritesheetWidth()
    {
        var sum = Hat.AllSprites.Sum(x => x.Sprite.Width * x.Sprite.Height);

        return RoundedUpPowerOfTwo(
            (int)Math.Ceiling(
                Math.Sqrt(sum)
            )
        );
    }

    private void LoadAllSprites()
    {
        foreach (var sprite in Hat.AllSprites)
        {
            sprite.Sprite.Load();
        }
    }

    public void Dispose()
    {
        foreach (var sprite in Hat.AllSprites)
        {
            sprite.Sprite.Dispose();
        }
    }

    private static int RoundedUpPowerOfTwo(int n)
    {
        var power = 1;
        while (power < n) power <<= 1;
        return power;
    }
}