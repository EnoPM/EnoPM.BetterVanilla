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
}