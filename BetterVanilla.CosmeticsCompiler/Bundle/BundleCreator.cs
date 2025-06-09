using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Bundle;
using BetterVanilla.Cosmetics.Api.Serialization;

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
        foreach (var hatPath in Options.HatSpritesheet)
        {
            var hat = JsonSerializer.Deserialize<SerializedHat>(File.ReadAllText(hatPath));
            if (hat == null)
            {
                throw new Exception("Unable to deserialize: " + hatPath);
            }
            bundle.AddHat(hat);
        }

        using var file = File.Create(Options.OutputFilePath);
        
        bundle.Serialize(file, Options.EnableCompression);
    }
}