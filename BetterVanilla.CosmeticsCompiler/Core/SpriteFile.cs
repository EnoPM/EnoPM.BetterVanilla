using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetterVanilla.CosmeticsCompiler.Core;

public class SpriteFile
{
    public string Path { get; set; }
    public LoadableSprite Sprite { get; }

    public SpriteFile(string filePath, string name)
    {
        Path = filePath;
        Sprite = new LoadableSprite(filePath, $"{name}");
    }

    public SpriteFile(string name, Image<Rgba32> image)
    {
        Path = name;
        Sprite = new LoadableSprite(name, image);
    }
}