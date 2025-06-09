using SixLabors.ImageSharp;

namespace BetterVanilla.CosmeticsCompiler.Core;

public sealed class LoadableSprite( string filePath, string name) : IDisposable
{
    public string Name { get; set; } = name;
    public Image? Image { get; set; }
    public int X { get; set; } = -1;
    public int Y { get; set; } = -1;
    public int Width => Image?.Width ?? -1;
    public int Height => Image?.Height ?? -1;

    public void Load()
    {
        if (Image != null) return;
        Image = Image.Load(filePath);
    }

    public void Dispose()
    {
        Image?.Dispose();
    }
}