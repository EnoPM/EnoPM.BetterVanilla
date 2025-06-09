using CommandLine;
using JetBrains.Annotations;

namespace BetterVanilla.CosmeticsCompiler.Bundle;

[Verb("bundle"), UsedImplicitly]
public sealed class BundleOptions
{
    [Option('o', "output", Required = true, HelpText = "Output file path")]
    public string OutputFilePath { get; [UsedImplicitly] set; } = null!;
    
    [Option("compression", Default = false, HelpText = "Compress bundle")]
    public bool EnableCompression { get; [UsedImplicitly] set; }
    
    [Option("hats", HelpText = "Hats spritesheet json files")]
    public IEnumerable<string> HatSpritesheet { get; [UsedImplicitly] set; } = null!;
    
    [Option("visors", HelpText = "Hats spritesheet json files")]
    public IEnumerable<string> VisorSpritesheet { get; [UsedImplicitly] set; } = null!;
}