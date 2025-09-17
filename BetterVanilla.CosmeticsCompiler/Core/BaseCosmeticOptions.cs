namespace BetterVanilla.CosmeticsCompiler.Core;

public abstract class BaseCosmeticOptions
{
    public string OutputDirectoryPath { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? AuthorName { get; set; }
    public bool IsAdaptive { get; set; }
}