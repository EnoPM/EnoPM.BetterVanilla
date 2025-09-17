using System.CommandLine;
using BetterVanilla.CosmeticsCompiler.NamePlatesSpritesheet;
using BetterVanilla.ToolsLib.Utils;

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
        try
        {
            using var creator = new NamePlateSpritesheetCreator(options);
            creator.Process();
            
            ConsoleUtility.WriteLine(ConsoleColor.DarkGreen, $"NamePlate spritesheet manifest generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.spritesheet.json")}");
            ConsoleUtility.WriteLine(ConsoleColor.DarkGreen,$"NamePlate spritesheet file generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.png")}");
        }
        catch (Exception ex)
        {
            ConsoleUtility.WriteLine(ConsoleColor.Red,$"Error during nameplate creation  ({options.Name}): {ex.Message}");
        }
        ConsoleUtility.NewLine();
    }
}