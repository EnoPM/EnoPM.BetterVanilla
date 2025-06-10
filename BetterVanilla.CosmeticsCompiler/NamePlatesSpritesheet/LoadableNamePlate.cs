using BetterVanilla.Cosmetics.Api.Core.Serialization;
using BetterVanilla.Cosmetics.Api.NamePlates;
using BetterVanilla.CosmeticsCompiler.Core;

namespace BetterVanilla.CosmeticsCompiler.NamePlatesSpritesheet;

public sealed class LoadableNamePlate : INamePlate<SpriteFile>
{
    public string Name { get; set; }

    public bool Adaptive { get; set; }

    public SerializedCosmeticAuthor? Author { get; set; }
    
    public SpriteFile MainResource { get; set; }

    public List<SpriteFile> AllSprites { get; } = [];

    public LoadableNamePlate(CreateNameplateSpritesheetOptions options)
    {
        Name = options.Name;
        Adaptive = options.IsAdaptive;
        MainResource = CreateSpriteFile(nameof(MainResource), options.MainResourceFilePath);
        if (options.AuthorName != null)
        {
            Author = new SerializedCosmeticAuthor
            {
                Name = options.AuthorName
            };
        }
    }

    private SpriteFile CreateSpriteFile(string resourceName, string resourcePath)
    {
        if (!File.Exists(resourcePath))
        {
            throw new FileNotFoundException($"Could not find resource '{resourcePath}'");
        }
        var existing = AllSprites
            .FirstOrDefault(x => x.Path == resourcePath);
        if (existing != null)
        {
            return existing;
        }
        var name = $"{Name}_{resourceName}";
        var spriteFile = new SpriteFile(resourcePath, name);
        AllSprites.Add(spriteFile);
        return spriteFile;
    }
}