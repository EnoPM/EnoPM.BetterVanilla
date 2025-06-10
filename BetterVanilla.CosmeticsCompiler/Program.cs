using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core.Bundle;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using BetterVanilla.CosmeticsCompiler.Bundle;
using BetterVanilla.CosmeticsCompiler.HatsSpritesheet;
using BetterVanilla.CosmeticsCompiler.NamePlatesSpritesheet;
using BetterVanilla.CosmeticsCompiler.VisorsSpritesheet;
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
        Parser.Default.ParseArguments<GenerateSchemaOptions, CreateHatSpritesheetOptions, CreateVisorSpritesheetOptions, CreateNameplateSpritesheetOptions, BundleOptions>(args)
            .WithParsed<GenerateSchemaOptions>(GenerateSchema)
            .WithParsed<CreateHatSpritesheetOptions>(CreateHatSpritesheet)
            .WithParsed<CreateVisorSpritesheetOptions>(CreateVisorSpritesheet)
            .WithParsed<CreateNameplateSpritesheetOptions>(CreateNamePlateSpritesheet)
            .WithParsed<BundleOptions>(Bundle)
            .WithNotParsed(HandleError);
    }

    private static void Bundle(BundleOptions options)
    {
        var creator = new BundleCreator(options);
        creator.Process();
        
        Console.WriteLine($"Bundle file generated at {options.OutputFilePath}");
    }

    private static void CreateNamePlateSpritesheet(CreateNameplateSpritesheetOptions options)
    {
        using var creator = new NamePlateSpritesheetCreator(options);
        creator.Process();
        Console.WriteLine($"NamePlate spritesheet file generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.png")}");
        Console.WriteLine($"NamePlate spritesheet manifest generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.spritesheet.json")}");
    }

    private static void CreateHatSpritesheet(CreateHatSpritesheetOptions options)
    {
        using var creator = new HatSpritesheetCreator(options);
        creator.Process();
        
        Console.WriteLine($"Hat spritesheet file generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.png")}");
        Console.WriteLine($"Hat spritesheet manifest generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.spritesheet.json")}");
    }
    
    private static void CreateVisorSpritesheet(CreateVisorSpritesheetOptions options)
    {
        using var creator = new VisorSpritesheetCreator(options);
        creator.Process();
        
        Console.WriteLine($"Visor spritesheet file generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.png")}");
        Console.WriteLine($"Visor spritesheet manifest generated at {Path.Combine(options.OutputDirectoryPath, $"{options.Name}.spritesheet.json")}");
    }

    private static void GenerateSchema(GenerateSchemaOptions options)
    {
        var builder = new JsonSchemaBuilder();
        var schema = builder.FromType<CosmeticBundle>().Build();
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