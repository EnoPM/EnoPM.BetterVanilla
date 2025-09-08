using System.IO;
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
        
        var hats = reader.ReadSerializedList<SerializedHat>(compressed);
        var visors = reader.ReadSerializedList<SerializedVisor>(compressed);
        var namePlates = reader.ReadSerializedList<SerializedNamePlate>(compressed);
        
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