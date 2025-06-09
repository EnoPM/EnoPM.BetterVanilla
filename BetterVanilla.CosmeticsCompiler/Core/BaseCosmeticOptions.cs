using CommandLine;

namespace BetterVanilla.CosmeticsCompiler.Core;

public abstract class BaseCosmeticOptions
{
    [Option('o', "output", Required = true, HelpText = "Output directory path")]
    public string OutputDirectoryPath { get; set; } = null!;
    
    [Option('n', "name", Required = true, HelpText = "Cosmetic name")]
    public string Name { get; set; } = null!;
    
    [Option("author", HelpText = "Cosmetic author name")]
    public string? AuthorName { get; set; }
    
    [Option("adaptive", Default = false, HelpText = "Is cosmetic adaptive")]
    public bool IsAdaptive { get; set; }
}