using System.Text;
using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Serialization;

namespace BetterVanilla.Cosmetics.Api.Bundle.Versions;

internal sealed class SerializerV1 : IBundleSerializer
{
    internal const int Version = 1;
    
    public CosmeticBundle Deserialize(BinaryReader reader)
    {
        var compressed = reader.ReadBoolean();
        var bundle = new CosmeticBundle();

        // Hat deserialization
        var jsonLen = reader.ReadInt32();
        var rawJsonBytes = reader.ReadBytes(jsonLen);
        var jsonBytes = compressed ? BundleSerializer.Decompress(rawJsonBytes) : rawJsonBytes;
        var json = Encoding.UTF8.GetString(jsonBytes);
        bundle.Hats = JsonSerializer.Deserialize<List<SerializedHat>>(json) ?? [];

        // Spritesheet deserialization
        var fileCount = reader.ReadInt32();

        for (var i = 0; i < fileCount; i++)
        {
            var nameLen = reader.ReadInt32();
            var fileName = Encoding.UTF8.GetString(reader.ReadBytes(nameLen));
            var dataLen = reader.ReadInt32();
            var rawData = reader.ReadBytes(dataLen);

            var data = compressed ? BundleSerializer.Decompress(rawData) : rawData;

            bundle.AllSpritesheet.Add(fileName, data);
        }

        return bundle;
    }
}