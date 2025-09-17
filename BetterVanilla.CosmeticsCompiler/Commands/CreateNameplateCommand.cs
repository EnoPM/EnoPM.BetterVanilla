using System.CommandLine;
using BetterVanilla.CosmeticsCompiler.NamePlatesSpritesheet;

namespace BetterVanilla.CosmeticsCompiler.Commands;

public sealed class CreateNameplateCommand : BaseSpritesheetCommand<CreateNameplateSpritesheetOptions>
{
    private Option<string> MainResourceFilePath { get; }
    
    public CreateNameplateCommand() : base("create-nameplate-spritesheet", "Create nameplate spritesheet")
    {
        MainResourceFilePath = CreateOption<string>("main-resource", "Nameplate main resource file path", true);
        
        Command.Add(MainResourceFilePath);
    }
    
    protected override CreateNameplateSpritesheetOptions ParseArguments(ParseResult result)
    {
        return new CreateNameplateSpritesheetOptions
        {
            OutputDirectoryPath = result.GetRequiredValue(OutputDirectoryPath),
            Name = result.GetRequiredValue(Name),
            AuthorName = result.GetValue(AuthorName),
            IsAdaptive = result.GetValue(IsAdaptive) ?? false,
            MainResourceFilePath = result.GetRequiredValue(MainResourceFilePath),
        };
    }
    
    protected override void Execute(CreateNameplateSpritesheetOptions options)
    {
        using var creator = new NamePlateSpritesheetCreator(options);
        creator.Process();
        Console.WriteLine($"NamePlate spritesheet file generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.png")}");
        Console.WriteLine($"NamePlate spritesheet manifest generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.spritesheet.json")}");
    }
}