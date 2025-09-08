using System.Text;
using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core.Bundle;
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
        if (args.Length == 1 && args[0].StartsWith('@'))
        {
            var path = args[0][1..].Trim('"');
            Console.WriteLine($"Running from file {path}");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found", path);
            }

            var lines = File.ReadAllLines(path);
            var current = new List<string>();
            foreach (var line in lines)
            {
                if (line.StartsWith('#')) continue;
                if (string.IsNullOrWhiteSpace(line))
                {
                    RunIfNotEmpty(current);
                    current.Clear();
                }
                else
                {
                    current.AddRange(SplitCommandLine(line));
                }
            }
            RunIfNotEmpty(current);
        }
        else
        {
            Run(args);
        }
    }
    
    private static void RunIfNotEmpty(List<string> args)
    {
        if (args.Count <= 0) return;
        Run(args.ToArray());
    }
    
    private static IEnumerable<string> SplitCommandLine(string commandLine)
    {
        if (string.IsNullOrWhiteSpace(commandLine))
        {
            return [];
        }

        var args = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        foreach (var c in commandLine)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (current.Length <= 0) continue;
                args.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
        {
            args.Add(current.ToString());
        }

        return args;
    }

    private static void Run(string[] args)
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