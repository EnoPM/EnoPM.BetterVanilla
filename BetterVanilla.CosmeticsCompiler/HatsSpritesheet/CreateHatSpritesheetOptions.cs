using BetterVanilla.CosmeticsCompiler.Core;

namespace BetterVanilla.CosmeticsCompiler.HatsSpritesheet;

public sealed class CreateHatSpritesheetOptions : BaseCosmeticOptions
{
    public bool IsBounce { get; set; }
    public bool NoVisors { get; set; }
    public string MainResourceFilePath { get; set; } = null!;
    public string? FlipResourceFilePath { get; set; }
    public string? BackResourceFilePath { get; set; }
    public string? BackFlipResourceFilePath { get; set; }
    public string? ClimbResourceFilePath { get; set; }
    public IEnumerable<string> FrontAnimationFrameFilePaths { get; set; } = null!;
    public IEnumerable<string> BackAnimationFrameFilePaths { get; set; } = null!;
}