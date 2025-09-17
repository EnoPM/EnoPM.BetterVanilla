using System.CommandLine;
using BetterVanilla.CosmeticsCompiler.Bundle;
using BetterVanilla.ToolsLib.Utils;

namespace BetterVanilla.CosmeticsCompiler.Commands;

public sealed class BundleCommand : BaseCommand<BundleOptions>
{
    private Option<string> OutputFilePath { get; }
    private Option<bool?> EnableCompression { get; }
    private Option<string[]?> HatSpritesheet { get; }
    private Option<string[]?> VisorSpritesheet { get; }
    private Option<string[]?> NameplateSpritesheet { get; }
    
    public BundleCommand() : base("bundle", "Create cosmetics bundle")
    {
        OutputFilePath = CreateOption<string>("output", "Output file path", true, "-o");
        EnableCompression = CreateOption<bool?>("compression", "Compress bundle");
        HatSpritesheet = CreateMultipleOption<string>("hats", "Hats spritesheet json files");
        VisorSpritesheet = CreateMultipleOption<string>("visors", "Visors spritesheet json files");
        NameplateSpritesheet = CreateMultipleOption<string>("nameplates", "Nameplates spritesheet json files");
        
        Command.Add(OutputFilePath);
        Command.Add(EnableCompression);
        Command.Add(HatSpritesheet);
        Command.Add(VisorSpritesheet);
        Command.Add(NameplateSpritesheet);
    }

    protected override BundleOptions ParseArguments(ParseResult result)
    {
        return new BundleOptions
        {
            OutputFilePath = result.GetRequiredValue(OutputFilePath),
            EnableCompression = result.GetValue(EnableCompression) ?? false,
            HatSpritesheet = result.GetValue(HatSpritesheet) ?? [],
            VisorSpritesheet = result.GetValue(VisorSpritesheet) ?? [],
            NameplateSpritesheet = result.GetValue(NameplateSpritesheet) ?? [],
        };
    }
    
    protected override void Execute(BundleOptions options)
    {
        try
        {
            var creator = new BundleCreator(options);
            creator.Process();
            
            ConsoleUtility.WriteLine(ConsoleColor.Green, $"Bundle file generated at {options.OutputFilePath}");
        }
        catch (Exception ex)
        {
            ConsoleUtility.WriteLine(ConsoleColor.Red,$"Error during bundle creation: {ex.Message}");
        }
        ConsoleUtility.NewLine();
    }
}