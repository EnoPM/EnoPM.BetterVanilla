using BetterVanilla.CosmeticsCompiler.Core;
using CommandLine;
using JetBrains.Annotations;

namespace BetterVanilla.CosmeticsCompiler.NamePlatesSpritesheet;

[Verb("create-nameplate-spritesheet"), UsedImplicitly]
public sealed class CreateNameplateSpritesheetOptions : BaseCosmeticOptions
{
    [Option("main-resource", Required = true, HelpText = "Visor main resource file path")]
    public string MainResourceFilePath { get; [UsedImplicitly] set; } = null!;
}