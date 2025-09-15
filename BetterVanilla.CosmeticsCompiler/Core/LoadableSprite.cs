using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetterVanilla.CosmeticsCompiler.Core;

public sealed class LoadableSprite : IDisposable
{
    public string Name { get; set; }
    public Image<Rgba32>? Image { get; set; }
    public int X { get; set; } = -1;
    public int Y { get; set; } = -1;
    public int Width => Image?.Width ?? -1;
    public int Height => Image?.Height ?? -1;
    
    private string? ResourcePath { get; set; }

    public void Load()
    {
        if (Image != null) return;
        if (ResourcePath == null)
        {
            throw new NullReferenceException($"{nameof(ResourcePath)} cannot be null for unloaded sprite.");
        }
        Image = SixLabors.ImageSharp.Image.Load<Rgba32>(ResourcePath);
    }

    public LoadableSprite(string filePath, string name)
    {
        Name = name;
        ResourcePath = filePath;
    }

    public LoadableSprite(string name, Image<Rgba32> image)
    {
        Name = name;
        Image = image;
    }

    public void Dispose()
    {
        Image?.Dispose();
    }
}