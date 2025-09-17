using System.CommandLine;
using BetterVanilla.CosmeticsCompiler.VisorsSpritesheet;
using BetterVanilla.ToolsLib.Utils;

namespace BetterVanilla.CosmeticsCompiler.Commands;

public sealed class CreateVisorCommand : BaseSpritesheetCommand<CreateVisorSpritesheetOptions>
{
    private Option<bool?> IsBehindHats { get; }
    private Option<string> MainResourceFilePath { get; }
    private Option<string?> LeftResourceFilePath { get; }
    private Option<string?> ClimbResourceFilePath { get; }
    private Option<string?> FloorResourceFilePath { get; }
    private Option<string[]?> FrontAnimationFrameFilePaths { get; }

    public CreateVisorCommand() : base("create-visor-spritesheet", "Create visor spritesheet")
    {
        IsBehindHats = CreateOption<bool?>("behind-hat", "Is behind hats");
        MainResourceFilePath = CreateOption<string>("main-resource", "Visor main resource file path", true);

        LeftResourceFilePath = CreateOption<string?>("left-resource", "Visor left resource file path");
        ClimbResourceFilePath = CreateOption<string?>("climb-resource", "Visor climb resource file path");
        FloorResourceFilePath = CreateOption<string?>("floor-resource", "Visor floor resource file path");
        FrontAnimationFrameFilePaths = CreateMultipleOption<string>("front-animation-frames", "Front animation frame resource file paths");

        Command.Add(IsBehindHats);
        Command.Add(MainResourceFilePath);
        Command.Add(LeftResourceFilePath);
        Command.Add(ClimbResourceFilePath);
        Command.Add(FloorResourceFilePath);
        Command.Add(FrontAnimationFrameFilePaths);
    }

    protected override CreateVisorSpritesheetOptions ParseArguments(ParseResult result)
    {
        return new CreateVisorSpritesheetOptions
        {
            OutputDirectoryPath = result.GetRequiredValue(OutputDirectoryPath),
            Name = result.GetRequiredValue(Name),
            AuthorName = result.GetValue(AuthorName),
            IsAdaptive = result.GetValue(IsAdaptive) ?? false,
            IsBehindHats = result.GetValue(IsBehindHats) ?? false,
            MainResourceFilePath = result.GetRequiredValue(MainResourceFilePath),
            LeftResourceFilePath = result.GetValue(LeftResourceFilePath),
            ClimbResourceFilePath = result.GetValue(ClimbResourceFilePath),
            FloorResourceFilePath = result.GetValue(FloorResourceFilePath),
            FrontAnimationFrameFilePaths = result.GetValue(FrontAnimationFrameFilePaths) ?? []
        };
    }

    protected override void Execute(CreateVisorSpritesheetOptions options)
    {
        try
        {
            using var creator = new VisorSpritesheetCreator(options);
            creator.Process();

            ConsoleUtility.WriteLine(ConsoleColor.DarkGreen, $"Visor spritesheet file generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.png")}");
            ConsoleUtility.WriteLine(ConsoleColor.DarkGreen, $"Visor spritesheet manifest generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.spritesheet.json")}");
        }
        catch (Exception ex)
        {
            ConsoleUtility.WriteLine(ConsoleColor.Red,$"Error during visor creation ({options.Name}): {ex.Message}");
        }
        ConsoleUtility.NewLine();
    }
}