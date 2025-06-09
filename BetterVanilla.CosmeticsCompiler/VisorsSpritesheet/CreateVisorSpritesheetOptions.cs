using BetterVanilla.CosmeticsCompiler.Core;
using CommandLine;
using JetBrains.Annotations;

namespace BetterVanilla.CosmeticsCompiler.VisorsSpritesheet;

[Verb("create-visor-spritesheet"), UsedImplicitly]
public sealed class CreateVisorSpritesheetOptions : BaseCosmeticOptions
{
    [Option("behind-hat", Default = false, HelpText = "Is behind hats")]
    public bool IsBehindHats { get; [UsedImplicitly] set; }

    [Option("main-resource", Required = true, HelpText = "Visor main resource file path")]
    public string MainResourceFilePath { get; [UsedImplicitly] set; } = null!;
    
    [Option("left-resource", HelpText = "Visor left resource file path")]
    public string? LeftResourceFilePath { get; [UsedImplicitly] set; }
    
    [Option("climb-resource", HelpText = "Visor back resource file path")]
    public string? ClimbResourceFilePath { get; [UsedImplicitly] set; }
    
    [Option("floor-resource", HelpText = "Visor floor resource file path")]
    public string? FloorResourceFilePath { get; [UsedImplicitly] set; }
}