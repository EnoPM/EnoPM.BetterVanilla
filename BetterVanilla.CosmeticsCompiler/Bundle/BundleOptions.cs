namespace BetterVanilla.CosmeticsCompiler.Bundle;

public sealed class BundleOptions
{
    public string OutputFilePath { get; set; } = null!;
    public bool EnableCompression { get; set; }
    public IEnumerable<string> HatSpritesheet { get; set; } = null!;
    public IEnumerable<string> VisorSpritesheet { get; set; } = null!;
    public IEnumerable<string> NameplateSpritesheet { get; set; } = null!;
}