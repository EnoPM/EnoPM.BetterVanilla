using System.CommandLine;
using BetterVanilla.CosmeticsCompiler.HatsSpritesheet;
using BetterVanilla.ToolsLib.Utils;

namespace BetterVanilla.CosmeticsCompiler.Commands;

public sealed class CreateHatCommand : BaseSpritesheetCommand<CreateHatSpritesheetOptions>
{
    private Option<bool?> IsBounce { get; }
    private Option<bool?> NoVisors { get; }
    private Option<string> MainResourceFilePath { get; }
    private Option<string?> FlipResourceFilePath { get; }
    private Option<string?> BackResourceFilePath { get; }
    private Option<string?> BackFlipResourceFilePath { get; }
    private Option<string?> ClimbResourceFilePath { get; }
    private Option<string[]?> FrontAnimationFrameFilePaths { get; }
    private Option<string[]?> BackAnimationFrameFilePaths { get; }
    
    public CreateHatCommand() : base("create-hat-spritesheet", "Create hat spritesheet")
    {
        IsBounce = CreateOption<bool?>("bounce", "Is hat bounce");
        NoVisors = CreateOption<bool?>("no-visors", "If hat cannot have visors");
        MainResourceFilePath = CreateOption<string>("main-resource", "Hat main resource file path", true);
        FlipResourceFilePath = CreateOption<string?>("flip-resource", "Hat flip resource file path");
        BackResourceFilePath = CreateOption<string?>("back-resource", "Hat back resource file path");
        BackFlipResourceFilePath = CreateOption<string?>("back-flip-resource", "Hat back flip resource file path");
        ClimbResourceFilePath = CreateOption<string?>("climb-resource", "Hat climb resource file path");
        FrontAnimationFrameFilePaths = CreateMultipleOption<string>("front-animation-frames", "Front animation frame resource file paths");
        BackAnimationFrameFilePaths = CreateMultipleOption<string>("back-animation-frames", "Back animation frame resource file paths");
        
        Command.Add(IsBounce);
        Command.Add(NoVisors);
        Command.Add(MainResourceFilePath);
        Command.Add(FlipResourceFilePath);
        Command.Add(BackResourceFilePath);
        Command.Add(BackFlipResourceFilePath);
        Command.Add(ClimbResourceFilePath);
        Command.Add(FrontAnimationFrameFilePaths);
        Command.Add(BackAnimationFrameFilePaths);
    }
    
    protected override CreateHatSpritesheetOptions ParseArguments(ParseResult result)
    {
        return new CreateHatSpritesheetOptions
        {
            OutputDirectoryPath = result.GetRequiredValue(OutputDirectoryPath),
            Name = result.GetRequiredValue(Name),
            AuthorName = result.GetValue(AuthorName),
            IsAdaptive = result.GetValue(IsAdaptive) ?? false,
            IsBounce = result.GetValue(IsBounce) ?? false,
            NoVisors = result.GetValue(NoVisors) ?? false,
            MainResourceFilePath = result.GetRequiredValue(MainResourceFilePath),
            FlipResourceFilePath = result.GetValue(FlipResourceFilePath),
            BackResourceFilePath = result.GetValue(BackResourceFilePath),
            BackFlipResourceFilePath = result.GetValue(BackFlipResourceFilePath),
            ClimbResourceFilePath = result.GetValue(ClimbResourceFilePath),
            FrontAnimationFrameFilePaths = result.GetValue(FrontAnimationFrameFilePaths) ?? [],
            BackAnimationFrameFilePaths = result.GetValue(BackAnimationFrameFilePaths) ?? []
        };
    }
    
    protected override void Execute(CreateHatSpritesheetOptions options)
    {
        try
        {
            using var creator = new HatSpritesheetCreator(options);
            creator.Process();

            ConsoleUtility.WriteLine(ConsoleColor.DarkGreen,$"Hat spritesheet file generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.png")}");
            ConsoleUtility.WriteLine(ConsoleColor.DarkGreen,$"Hat spritesheet manifest generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.spritesheet.json")}");
        }
        catch (Exception ex)
        {
            ConsoleUtility.WriteLine(ConsoleColor.Red,$"Error during hat creation ({options.Name}): {ex.Message}");
        }
        ConsoleUtility.NewLine();
    }
}