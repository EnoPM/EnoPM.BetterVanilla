using CommandLine;
using JetBrains.Annotations;

namespace BetterVanilla.CosmeticsCompiler;

[Verb("generate-schema"), UsedImplicitly]
public sealed class GenerateSchemaOptions
{
    [Option('o', "output", Required = true, HelpText = "json schema output path")]
    public string OutputFilePath { get; [UsedImplicitly] set; } = null!;
    
    [Option('p', "pretty", Default = false, HelpText = "readable json output")]
    public bool PrettyPrint { get; [UsedImplicitly] set; }
}