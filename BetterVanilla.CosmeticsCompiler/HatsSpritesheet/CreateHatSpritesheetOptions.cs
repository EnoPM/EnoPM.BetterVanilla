using BetterVanilla.CosmeticsCompiler.Core;
using CommandLine;
using JetBrains.Annotations;

namespace BetterVanilla.CosmeticsCompiler.HatsSpritesheet;

[Verb("create-hat-spritesheet"), UsedImplicitly]
public sealed class CreateHatSpritesheetOptions : BaseCosmeticOptions
{
    [Option("bounce", Default = false, HelpText = "Is hat bounce")]
    public bool IsBounce { get; [UsedImplicitly] set; }

    [Option("main-resource", Required = true, HelpText = "Hat main resource file path")]
    public string MainResourceFilePath { get; [UsedImplicitly] set; } = null!;
    
    [Option("flip-resource", HelpText = "Hat flip resource file path")]
    public string? FlipResourceFilePath { get; [UsedImplicitly] set; }
    
    [Option("back-resource", HelpText = "Hat back resource file path")]
    public string? BackResourceFilePath { get; [UsedImplicitly] set; }
    
    [Option("back-flip-resource", HelpText = "Hat back flip resource file path")]
    public string? BackFlipResourceFilePath { get; [UsedImplicitly] set; }
    
    [Option("climb-resource", HelpText = "Hat climb resource file path")]
    public string? ClimbResourceFilePath { get; [UsedImplicitly] set; }

    [Option("front-animation-frames", HelpText = "Front animation frame resource file paths")]
    public IEnumerable<string> FrontAnimationFrameFilePaths { get; [UsedImplicitly] set; } = null!;
    
    [Option("back-animation-frames", HelpText = "Front animation frame resource file paths")]
    public IEnumerable<string> BackAnimationFrameFilePaths { get; [UsedImplicitly] set; } = null!;
}