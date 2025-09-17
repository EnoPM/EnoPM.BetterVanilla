using System.CommandLine;
using BetterVanilla.CosmeticsCompiler.Core;

namespace BetterVanilla.CosmeticsCompiler.Commands;

public abstract class BaseSpritesheetCommand<TOption> : BaseCommand<TOption>
where TOption : BaseCosmeticOptions
{
    protected Option<string> OutputDirectoryPath { get; }
    protected Option<string> Name { get; }
    protected Option<string?> AuthorName { get; }
    protected Option<bool?> IsAdaptive { get; }
    
    protected BaseSpritesheetCommand(string name, string description) : base(name, description)
    {
        OutputDirectoryPath = CreateOption<string>("output", "Output directory path", true, "-o");
        Name = CreateOption<string>("name", "Cosmetic name", true, "-n");
        AuthorName = CreateOption<string?>("author", "Cosmetic author name");
        IsAdaptive = CreateOption<bool?>("adaptive", "Is cosmetic adaptive");
        
        Command.Add(OutputDirectoryPath);
        Command.Add(Name);
        Command.Add(AuthorName);
        Command.Add(IsAdaptive);
    }
}