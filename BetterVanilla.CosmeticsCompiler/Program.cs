using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Serialization;
using BetterVanilla.CosmeticsCompiler.Bundle;
using BetterVanilla.CosmeticsCompiler.CosmeticsSpritesheet;
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
        Parser.Default.ParseArguments<GenerateSchemaOptions, CreateHatSpritesheetOptions, BundleOptions>(args)
            .WithParsed<GenerateSchemaOptions>(GenerateSchema)
            .WithParsed<CreateHatSpritesheetOptions>(CreateHatSpritesheet)
            .WithParsed<BundleOptions>(Bundle)
            .WithNotParsed(HandleError);
    }

    private static void Bundle(BundleOptions options)
    {
        var creator = new BundleCreator(options);
        creator.Process();
        
        Console.WriteLine($"Bundle file generated at {options.OutputFilePath}");
    }

    private static void CreateHatSpritesheet(CreateHatSpritesheetOptions options)
    {
        using var creator = new HatSpritesheetCreator(options);
        creator.Process();
        
        Console.WriteLine($"Hat spritesheet file generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.png")}");
        Console.WriteLine($"Hat spritesheet manifest generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.spritesheet.json")}");
    }

    private static void GenerateSchema(GenerateSchemaOptions options)
    {
        var builder = new JsonSchemaBuilder();
        var schema = builder.FromType<SerializedCosmeticsManifest>().Build();
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