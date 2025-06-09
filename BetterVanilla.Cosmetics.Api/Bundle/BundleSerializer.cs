using System.IO.Compression;
using System.Text;
using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Bundle.Versions;

namespace BetterVanilla.Cosmetics.Api.Bundle;

public static class BundleSerializer
{
    private const int MinVersion = SerializerV1.Version;
    private const int CurrentVersion = SerializerV1.Version;

    public static void SerializeBundle(CosmeticBundle bundle, Stream stream, BundleSerializerOptions options)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);

        writer.Write(CurrentVersion);
        writer.Write(options.Compressed);
        
        // Hats serialization
        var json = JsonSerializer.Serialize(bundle.Hats);
        var rawJsonBytes = Encoding.UTF8.GetBytes(json);
        var jsonBytes = options.Compressed ? Compress(rawJsonBytes) : rawJsonBytes;

        writer.Write(jsonBytes.Length);
        writer.Write(jsonBytes);

        // Spritesheet serialization
        writer.Write(bundle.AllSpritesheet.Count);

        foreach (var (fileName, data) in bundle.AllSpritesheet)
        {
            var nameBytes = Encoding.UTF8.GetBytes(fileName);
            writer.Write(nameBytes.Length);
            writer.Write(nameBytes);

            var dataToWrite = options.Compressed ? Compress(data) : data;

            writer.Write(dataToWrite.Length);
            writer.Write(dataToWrite);
        }
    }

    private static IBundleSerializer GetDeserializer(int version)
    {
        if (version < MinVersion)
        {
            throw new NotSupportedException($"The version {version} is not supported.");
        }
        if (version > CurrentVersion)
        {
            throw new NotSupportedException($"The version {version} doesn't exists.");
        }

        return version switch
        {
            1 => new SerializerV1(),
            _ => throw new NotSupportedException($"Unknown bundle version {version}.")
        };
    }

    public static CosmeticBundle DeserializeBundle(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: false);
        var version = reader.ReadInt32();
        
        var deserializer = GetDeserializer(version);
        
        return deserializer.Deserialize(reader);
    }
    
    internal static byte[] Compress(byte[] data)
    {
        using var ms = new MemoryStream();
        using (var brotli = new BrotliStream(ms, CompressionLevel.SmallestSize, leaveOpen: true))
            brotli.Write(data, 0, data.Length);
        return ms.ToArray();
    }

    internal static byte[] Decompress(byte[] data)
    {
        using var input = new MemoryStream(data);
        using var brotli = new BrotliStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        brotli.CopyTo(output);
        return output.ToArray();
    }
}