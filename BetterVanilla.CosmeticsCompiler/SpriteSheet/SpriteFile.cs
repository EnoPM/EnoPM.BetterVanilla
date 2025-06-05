using SixLabors.ImageSharp;

namespace BetterVanilla.CosmeticsCompiler.SpriteSheet;

public class SpriteFile
{
    public required string Path { get; set; }
    public Image? Image { get; set; }
}