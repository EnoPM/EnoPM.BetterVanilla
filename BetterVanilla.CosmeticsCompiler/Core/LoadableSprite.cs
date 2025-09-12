using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetterVanilla.CosmeticsCompiler.Core;

public sealed class LoadableSprite(string filePath, string name) : IDisposable
{
    public string Name { get; set; } = name;
    public Image<Rgba32>? Image { get; set; }
    public int X { get; set; } = -1;
    public int Y { get; set; } = -1;
    public int Width => Image?.Width ?? -1;
    public int Height => Image?.Height ?? -1;

    public void Load()
    {
        if (Image != null) return;
        Image = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);
    }

    public void Dispose()
    {
        Image?.Dispose();
    }
}