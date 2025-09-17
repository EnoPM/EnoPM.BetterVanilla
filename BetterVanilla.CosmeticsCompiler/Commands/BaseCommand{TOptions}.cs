using System.CommandLine;

namespace BetterVanilla.CosmeticsCompiler.Commands;

public abstract class BaseCommand<TOptions> : BaseCommand where TOptions : class
{
    protected BaseCommand(string name, string? description) : base(name, description)
    {
        
    }

    public override void Compile()
    {
        Command.SetAction(CommandAction);
    }

    private void CommandAction(ParseResult result)
    {
        var options = ParseArguments(result);
        Execute(options);
    }

    protected abstract TOptions ParseArguments(ParseResult result);
    
    protected abstract void Execute(TOptions options);
}