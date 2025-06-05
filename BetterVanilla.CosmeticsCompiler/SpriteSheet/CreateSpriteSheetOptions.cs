using CommandLine;

namespace BetterVanilla.CosmeticsCompiler.SpriteSheet;

[Verb("create-spritesheet")]
public sealed class CreateSpriteSheetOptions
{
    [Option('o', "output-path", Required = true, HelpText = "spritesheet output directory path")]
    public string OutputDirectoryPath { get; set; } = null!;
    
    [Option('n', "name", Required = true, HelpText = "spritesheet name")]
    public string Name { get; set; } = null!;
    
    [Option('f', "file-input", HelpText = "input png file paths")]
    public IEnumerable<string>? InputPngFilePaths { get; set; }
    
    [Option('d', "directory-input", HelpText = "input png directory paths")]
    public IEnumerable<string>? InputPngDirectoryPaths { get; set; }
}