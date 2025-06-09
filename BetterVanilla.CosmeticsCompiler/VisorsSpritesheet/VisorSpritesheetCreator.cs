using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using BetterVanilla.Cosmetics.Api.Visors;
using BetterVanilla.CosmeticsCompiler.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BetterVanilla.CosmeticsCompiler.VisorsSpritesheet;

public sealed class VisorSpritesheetCreator : IDisposable
{
    private CreateVisorSpritesheetOptions Options { get; }
    private LoadableVisor Visor { get; }
    private string SpritesheetOutputPath { get; }
    private string HatManifestOutputPath { get; }

    public VisorSpritesheetCreator(CreateVisorSpritesheetOptions options)
    {
        Options = options;
        Visor = new LoadableVisor(Options);
        SpritesheetOutputPath = Path.Combine(Options.OutputDirectoryPath, Visor.Name + ".png");
        HatManifestOutputPath = Path.Combine(Options.OutputDirectoryPath, Visor.Name + ".spritesheet.json");
    }

    public void Process()
    {
        LoadAllSprites();
        
        CalculateSpritePositions();
        
        using var spritesheet = DrawSpritesheet();
        spritesheet.Save(SpritesheetOutputPath);
        
        var manifest = Serialize(Visor);
        var serializedManifest = JsonSerializer.Serialize(manifest);
        File.WriteAllText(HatManifestOutputPath, serializedManifest);
    }

    private Image<Rgba32> DrawSpritesheet()
    {
        var width = Visor.AllSprites.Max(x => x.Sprite.X + x.Sprite.Width);
        var height = Visor.AllSprites.Max(x => x.Sprite.Y + x.Sprite.Height);

        var spriteSheet = new Image<Rgba32>(width, height);
        foreach (var spriteFile in Visor.AllSprites)
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

    private SerializedVisor Serialize(LoadableVisor visor)
    {
        return new SerializedVisor
        {
            Name = visor.Name,
            Adaptive = visor.Adaptive,
            Author = visor.Author,
            BehindHats = visor.BehindHats,
            MainResource = Serialize(visor.MainResource),
            LeftResource = visor.LeftResource == null ? null : Serialize(visor.LeftResource),
            ClimbResource = visor.ClimbResource == null ? null : Serialize(visor.ClimbResource),
            FloorResource = visor.FloorResource == null ? null : Serialize(visor.FloorResource)
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
        return Visor.AllSprites
            .OrderByDescending(x => x.Sprite.Height)
            .ToList();
    }

    private int CalculateMaxSpritesheetWidth()
    {
        var sum = Visor.AllSprites.Sum(x => x.Sprite.Width * x.Sprite.Height);

        return RoundedUpPowerOfTwo(
            (int)Math.Ceiling(
                Math.Sqrt(sum)
            )
        );
    }

    private void LoadAllSprites()
    {
        foreach (var sprite in Visor.AllSprites)
        {
            sprite.Sprite.Load();
        }
    }

    public void Dispose()
    {
        foreach (var sprite in Visor.AllSprites)
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