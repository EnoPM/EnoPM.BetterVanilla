using CommandLine;

namespace BetterVanilla.CosmeticsCompiler;

[Verb("generate-schema")]
public sealed class GenerateSchemaOptions
{
    [Option('o', "output", Required = true, HelpText = "json schema output path")]
    public string OutputFilePath { get; set; } = null!;
    
    [Option('p', "pretty", Default = false, HelpText = "readable json output")]
    public bool PrettyPrint { get; set; }
}