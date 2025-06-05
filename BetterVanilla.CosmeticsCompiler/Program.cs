using System.Text.Json;
using BetterVanilla.Cosmetics.Api;
using BetterVanilla.CosmeticsCompiler.SpriteSheet;
using CommandLine;
using Json.More;
using Json.Schema;
using Json.Schema.Generation;

namespace BetterVanilla.CosmeticsCompiler;

internal static class Program
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<GenerateSchemaOptions, CreateSpriteSheetOptions>(args)
            .WithParsed<GenerateSchemaOptions>(GenerateSchema)
            .WithParsed<CreateSpriteSheetOptions>(CreateSpriteSheet)
            .WithNotParsed(HandleError);
    }
    
    private static void CreateSpriteSheet(CreateSpriteSheetOptions options)
    {
        var creator = new SpriteSheetCreator(options);
        creator.CreateSpriteSheet();
        creator.SaveSpriteSheet();
        creator.SaveSpriteSheetMetaTxt();
    }

    private static void GenerateSchema(GenerateSchemaOptions options)
    {
        var builder = new JsonSchemaBuilder();
        var schema = builder.FromType<CosmeticRegistry>().Build();
        var jsonDoc = schema.ToJsonDocument();
        SerializerOptions.WriteIndented = options.PrettyPrint;
        var jsonString = JsonSerializer.Serialize(jsonDoc, SerializerOptions);
        File.WriteAllText(options.OutputFilePath, jsonString);
        
        Console.WriteLine($"Schema generated at: {options.OutputFilePath}");
    }

    private static void HandleError(IEnumerable<Error> errs)
    {
    }
}