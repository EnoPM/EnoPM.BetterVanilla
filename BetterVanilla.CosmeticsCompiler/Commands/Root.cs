using System.CommandLine;

namespace BetterVanilla.CosmeticsCompiler.Commands;

public sealed class Root
{
    private RootCommand Command { get; } = new();

    public void AddSubcommand(BaseCommand command)
    {
        command.Compile();
        Command.Add(command.Command);
    }

    public void Execute(string[] args)
    {
        Command.Parse(args).Invoke();
    }
}