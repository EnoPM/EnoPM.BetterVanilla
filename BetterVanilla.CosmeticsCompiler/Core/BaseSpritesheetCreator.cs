using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BetterVanilla.CosmeticsCompiler.Core;

public abstract class BaseSpritesheetCreator<TOptions, TLoadableCosmetic, TSerializedCosmetic> : IDisposable
    where TOptions : BaseCosmeticOptions
    where TLoadableCosmetic : class, ICosmeticItem
    where TSerializedCosmetic : SerializedCosmetic
{
    private TOptions Options { get; }
    protected TLoadableCosmetic Cosmetic { get; }
    private string SpritesheetOutputPath { get; }
    private string ManifestOutputPath { get; }

    protected BaseSpritesheetCreator(TOptions options, TLoadableCosmetic cosmetic)
    {
        Options = options;
        Cosmetic = cosmetic;
        SpritesheetOutputPath = Path.Combine(Options.OutputDirectoryPath, Cosmetic.Name + ".png");
        ManifestOutputPath = Path.Combine(Options.OutputDirectoryPath, Cosmetic.Name + ".spritesheet.json");
    }
    
    protected abstract TSerializedCosmetic Serialize(TLoadableCosmetic cosmetic);
    protected abstract string SerializeToJson(TSerializedCosmetic cosmetic);
    protected abstract List<SpriteFile> GetAllSprites();
    
    public void Process()
    {
        LoadAllSprites();
        
        DeduplicateSprites();
        
        CalculateSpritePositions();
        
        using var spritesheet = DrawSpritesheet();
        spritesheet.Save(SpritesheetOutputPath);
        
        var manifest = Serialize(Cosmetic);
        var serializedManifest = SerializeToJson(manifest);
        File.WriteAllText(ManifestOutputPath, serializedManifest);
    }
    
    private void DeduplicateSprites()
    {
        var cache = new Dictionary<string, SpriteFile>();

        foreach (var spriteFile in GetAllSprites())
        {
            var image = spriteFile.Sprite.Image;
            if (image == null)
            {
                throw new Exception($"Sprite not loaded: {spriteFile.Sprite.Name} - {spriteFile.Path}");
            }

            var hash = ComputeImageHash(image);

            if (cache.TryGetValue(hash, out var existing))
            {
                spriteFile.Sprite.Image?.Dispose();
                spriteFile.Sprite.Image = existing.Sprite.Image;
            }
            else
            {
                cache[hash] = spriteFile;
            }
        }
    }

    private List<SpriteFile> GetAllUniqueSprites()
    {
        var uniqueSprites = new List<SpriteFile>();
        foreach (var spriteFile in GetAllSprites())
        {
            var image = spriteFile.Sprite.Image;
            if (image == null)
            {
                throw new Exception($"Sprite not loaded: {spriteFile.Sprite.Name} - {spriteFile.Path}");
            }
            if (uniqueSprites.FirstOrDefault(x => x.Sprite.Image == image) != null)
            {
                continue;
            }
            uniqueSprites.Add(spriteFile);
        }
        return uniqueSprites;
    }
    
    private Image<Rgba32> DrawSpritesheet()
    {
        var uniqueSprites = GetAllUniqueSprites();
        var width = uniqueSprites.Max(x => x.Sprite.X + x.Sprite.Width);
        var height = uniqueSprites.Max(x => x.Sprite.Y + x.Sprite.Height);

        var spriteSheet = new Image<Rgba32>(width, height);
        foreach (var spriteFile in uniqueSprites)
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
    
    protected SerializedSprite Serialize(SpriteFile spriteFile)
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
        var cache = new Dictionary<Image<Rgba32>, (int x, int y)>();
        foreach (var file in spriteFiles)
        {
            var image = file.Sprite.Image;
            if (image == null)
            {
                throw new Exception($"Sprite not loaded: {file.Sprite.Name} - {file.Path}");
            }
            if (cache.TryGetValue(image, out var cached))
            {
                file.Sprite.X = cached.x;
                file.Sprite.Y = cached.y;
                continue;
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
            cache.Add(image, (file.Sprite.X, file.Sprite.Y));
        }
    }

    private List<SpriteFile> GetSortedSpriteFiles()
    {
        return GetAllSprites()
            .OrderByDescending(x => x.Sprite.Height)
            .ToList();
    }
    
    private int CalculateMaxSpritesheetWidth()
    {
        var sum = GetAllSprites().Sum(x => x.Sprite.Width * x.Sprite.Height);

        return RoundedUpPowerOfTwo(
            (int)Math.Ceiling(
                Math.Sqrt(sum)
            )
        );
    }

    private void LoadAllSprites()
    {
        foreach (var sprite in GetAllSprites())
        {
            sprite.Sprite.Load();
        }
    }
    
    public void Dispose()
    {
        foreach (var sprite in GetAllSprites())
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
    
    private static string ComputeImageHash(Image<Rgba32> image)
    {
        var hashCode = new HashCode();

        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (var x = 0; x < row.Length; x++)
                {
                    hashCode.Add(row[x].ToVector4());
                }
            }
        });

        return hashCode.ToHashCode().ToString("X");
    }
}