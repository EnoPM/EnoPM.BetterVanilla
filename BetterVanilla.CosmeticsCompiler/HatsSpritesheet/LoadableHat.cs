using BetterVanilla.Cosmetics.Api.Core.Serialization;
using BetterVanilla.Cosmetics.Api.Hats;
using BetterVanilla.CosmeticsCompiler.Core;

namespace BetterVanilla.CosmeticsCompiler.HatsSpritesheet;

public class LoadableHat : IHat<SpriteFile>
{
    public string Name { get; set; }

    public bool Adaptive { get; set; }

    public SerializedCosmeticAuthor? Author { get; set; }

    public SpriteFile MainResource { get; set; }
    
    public SpriteFile? FlipResource { get; set; }

    public SpriteFile? BackResource { get; set; }

    public SpriteFile? BackFlipResource { get; set; }

    public SpriteFile? ClimbResource { get; set; }

    public List<SpriteFile>? FrontAnimationFrames { get; set; }
    public List<SpriteFile>? BackAnimationFrames { get; set; }

    public bool Bounce { get; set; }
    
    public bool NoVisors { get; set; }

    public List<SpriteFile> AllSprites { get; } = [];

    public LoadableHat(CreateHatSpritesheetOptions options)
    {
        Name = options.Name;
        Adaptive = options.IsAdaptive;
        Bounce = options.IsBounce;
        NoVisors = options.NoVisors;
        MainResource = CreateSpriteFile(nameof(MainResource), options.MainResourceFilePath);
        if (options.AuthorName != null)
        {
            Author = new SerializedCosmeticAuthor
            {
                Name = options.AuthorName
            };
        }
        if (options.BackResourceFilePath != null)
        {
            BackResource = CreateSpriteFile(nameof(BackResource), options.BackResourceFilePath);
        }
        if (options.BackFlipResourceFilePath != null)
        {
            BackFlipResource = CreateSpriteFile(nameof(BackFlipResource), options.BackFlipResourceFilePath);
        }
        if (options.ClimbResourceFilePath != null)
        {
            ClimbResource = CreateSpriteFile(nameof(ClimbResource), options.ClimbResourceFilePath);
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

        var backAnimationFrames = options.BackAnimationFrameFilePaths.ToList();
        if (backAnimationFrames.Count != 0)
        {
            BackAnimationFrames = [];
            for (var i = 0; i < backAnimationFrames.Count; i++)
            {
                BackAnimationFrames.Add(CreateBackAnimationSpriteFile(backAnimationFrames[i], i));
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

    private SpriteFile CreateBackAnimationSpriteFile(string resourcePath, int frameIndex)
    {
        var name = $"AnimatedBackFrame_{frameIndex:000}";
        return CreateSpriteFile(name, resourcePath);
    }
}