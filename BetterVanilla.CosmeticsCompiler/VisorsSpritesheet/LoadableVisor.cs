using BetterVanilla.Cosmetics.Api.Core.Serialization;
using BetterVanilla.Cosmetics.Api.Visors;
using BetterVanilla.CosmeticsCompiler.Core;

namespace BetterVanilla.CosmeticsCompiler.VisorsSpritesheet;

public sealed class LoadableVisor : IVisor<SpriteFile>
{
    public string Name { get; set; }

    public bool Adaptive { get; set; }

    public SerializedCosmeticAuthor? Author { get; set; }
    
    public SpriteFile MainResource { get; set; }
    
    public SpriteFile? LeftResource { get; set; }
    
    public SpriteFile? ClimbResource { get; set; }
    
    public SpriteFile? FloorResource { get; set; }
    
    public List<SpriteFile>? FrontAnimationFrames { get; set; }

    public bool BehindHats { get; set; }

    public List<SpriteFile> AllSprites { get; } = [];

    public LoadableVisor(CreateVisorSpritesheetOptions options)
    {
        Name = options.Name;
        Adaptive = options.IsAdaptive;
        BehindHats = options.IsBehindHats;
        MainResource = CreateSpriteFile(nameof(MainResource), options.MainResourceFilePath);
        if (options.AuthorName != null)
        {
            Author = new SerializedCosmeticAuthor
            {
                Name = options.AuthorName
            };
        }
        if (options.LeftResourceFilePath != null)
        {
            LeftResource = CreateSpriteFile(nameof(LeftResource), options.LeftResourceFilePath);
        }
        if (options.ClimbResourceFilePath != null)
        {
            ClimbResource = CreateSpriteFile(nameof(ClimbResource), options.ClimbResourceFilePath);
        }
        if (options.FloorResourceFilePath != null)
        {
            FloorResource = CreateSpriteFile(nameof(FloorResource), options.FloorResourceFilePath);
        }
        
        var frontAnimationFrames = options.FrontAnimationFrameFilePaths.ToList();
        if (frontAnimationFrames.Count != 0)
        {
            FrontAnimationFrames = [];
            for (var i = 0; i < frontAnimationFrames.Count; i++)
            {
                FrontAnimationFrames.Add(CreateFrontAnimationSpriteFile(frontAnimationFrames[i], i));
            }
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
    
    private SpriteFile CreateFrontAnimationSpriteFile(string resourcePath, int frameIndex)
    {
        var name = $"AnimatedFrontFrame_{frameIndex:000}";
        return CreateSpriteFile(name, resourcePath);
    }
}