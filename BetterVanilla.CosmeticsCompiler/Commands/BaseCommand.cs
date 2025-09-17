using System.CommandLine;

namespace BetterVanilla.CosmeticsCompiler.Commands;

public abstract class BaseCommand
{
    public Command Command { get; }
    
    protected BaseCommand(string name, string? description = null)
    {
        Command = new Command(name, description);
    }
    
    protected static Option<T> CreateOption<T>(string name, string description, bool required = false, params string[] aliases)
    {
        return new Option<T>($"--{name}", aliases)
        {
            Description = description,
            Required = required
        };
    }

    public abstract void Compile();

    protected static Option<T> CreateOption<T>(string name, string description, params string[] aliases)
    {
        return CreateOption<T>(name, description, false, aliases);
    }
    
    protected static Option<T[]?> CreateMultipleOption<T>(string name, string description, bool required = false, params string[] aliases)
    {
        var option = CreateOption<T[]?>(name, description, required, aliases);
        option.AllowMultipleArgumentsPerToken = true;
        return option;
    }

    protected static Option<T[]?> CreateMultipleOption<T>(string name, string description, params string[] aliases)
    {
        return CreateMultipleOption<T>(name, description, false, aliases);
    }
}