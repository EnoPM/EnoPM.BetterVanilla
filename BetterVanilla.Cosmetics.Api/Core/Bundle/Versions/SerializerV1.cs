namespace BetterVanilla.Cosmetics.Api.Core.Bundle.Versions;

internal sealed class SerializerV1 : IBundleSerializer
{
    internal const int Version = 1;
    
    public CosmeticBundle Deserialize(BinaryReader reader)
    {
        var compressed = reader.ReadBoolean();
        
        var hats = reader.ReadHats(compressed);
        var visors = reader.ReadVisors(compressed);
        var allSpritesheet = reader.ReadAllSpritesheet(compressed);
        
        var bundle = new CosmeticBundle
        {
            Hats = hats,
            Visors = visors,
            AllSpritesheet = allSpritesheet
        };

        return bundle;
    }
}