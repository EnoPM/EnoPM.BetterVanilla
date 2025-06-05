using SixLabors.ImageSharp;

namespace BetterVanilla.CosmeticsCompiler.SpriteSheet;

public class SpriteEntry
{
    public string Name { get; set; } = string.Empty;
    public Image Image { get; set; } = null!;
    public int X { get; set; }
    public int Y { get; set; }
    public int Width => Image.Width;
    public int Height => Image.Height;
}
