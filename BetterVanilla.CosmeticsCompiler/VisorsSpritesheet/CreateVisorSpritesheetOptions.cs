using BetterVanilla.CosmeticsCompiler.Core;

namespace BetterVanilla.CosmeticsCompiler.VisorsSpritesheet;

public sealed class CreateVisorSpritesheetOptions : BaseCosmeticOptions
{
    public bool IsBehindHats { get; set; }
    public string MainResourceFilePath { get; set; } = null!;
    public string? LeftResourceFilePath { get; set; }
    public string? ClimbResourceFilePath { get; set; }
    public string? FloorResourceFilePath { get; set; }
    public IEnumerable<string> FrontAnimationFrameFilePaths { get; set; } = null!;
}