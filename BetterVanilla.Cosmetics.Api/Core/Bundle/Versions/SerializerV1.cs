using System.IO;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using BetterVanilla.Cosmetics.Api.Hats;
using BetterVanilla.Cosmetics.Api.NamePlates;
using BetterVanilla.Cosmetics.Api.Visors;

namespace BetterVanilla.Cosmetics.Api.Core.Bundle.Versions;

internal sealed class SerializerV1 : IBundleSerializer
{
    internal const int Version = 1;
    
    public CosmeticBundle Deserialize(BinaryReader reader)
    {
        var compressed = reader.ReadBoolean();
        
        var hats = reader.ReadSerializedList(CosmeticsJsonContext.Default.ListSerializedHat, compressed);
        var visors = reader.ReadSerializedList(CosmeticsJsonContext.Default.ListSerializedVisor, compressed);
        var namePlates = reader.ReadSerializedList(CosmeticsJsonContext.Default.ListSerializedNamePlate, compressed);
        
        var allSpritesheet = reader.ReadAllSpritesheet(compressed);
        
        var bundle = new CosmeticBundle
        {
            Hats = hats,
            Visors = visors,
            NamePlates = namePlates,
            AllSpritesheet = allSpritesheet
        };

        return bundle;
    }
}