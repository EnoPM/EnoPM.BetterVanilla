using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core.Bundle;
using BetterVanilla.Cosmetics.Api.Hats;
using BetterVanilla.Cosmetics.Api.NamePlates;
using BetterVanilla.Cosmetics.Api.Visors;

namespace BetterVanilla.CosmeticsCompiler.Bundle;

public sealed class BundleCreator
{
    private BundleOptions Options { get; }

    public BundleCreator(BundleOptions options)
    {
        Options = options;
    }

    public void Process()
    {
        var bundle = new CosmeticBundle();
        
        foreach (var cosmeticPath in Options.HatSpritesheet)
        {
            var hat = JsonSerializer.Deserialize<SerializedHat>(File.ReadAllText(cosmeticPath));
            if (hat == null)
            {
                throw new Exception("Unable to deserialize: " + cosmeticPath);
            }
            bundle.AddHat(hat);
        }
        
        foreach (var cosmeticPath in Options.VisorSpritesheet)
        {
            var visor = JsonSerializer.Deserialize<SerializedVisor>(File.ReadAllText(cosmeticPath));
            if (visor == null)
            {
                throw new Exception("Unable to deserialize: " + cosmeticPath);
            }
            bundle.AddVisor(visor);
        }
        
        foreach (var cosmeticPath in Options.NameplateSpritesheet)
        {
            var namePlate = JsonSerializer.Deserialize<SerializedNamePlate>(File.ReadAllText(cosmeticPath));
            if (namePlate == null)
            {
                throw new Exception("Unable to deserialize: " + cosmeticPath);
            }
            bundle.AddNamePlate(namePlate);
        }

        using var file = File.Create(Options.OutputFilePath);
        
        bundle.Serialize(file, Options.EnableCompression);
    }
}