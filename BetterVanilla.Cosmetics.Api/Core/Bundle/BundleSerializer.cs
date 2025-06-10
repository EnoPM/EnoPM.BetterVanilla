using System.IO.Compression;
using System.Text;
using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core.Bundle.Versions;

namespace BetterVanilla.Cosmetics.Api.Core.Bundle;

public static class BundleSerializer
{
    private const int MinVersion = SerializerV1.Version;
    private const int CurrentVersion = SerializerV1.Version;

    public static void SerializeBundle(CosmeticBundle bundle, Stream stream, BundleSerializerOptions options)
    {
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);

        writer.Write(CurrentVersion);
        writer.Write(options.Compressed);
        
        writer.WriteSerializedList(bundle.Hats, options.Compressed);
        writer.WriteSerializedList(bundle.Visors, options.Compressed);
        writer.WriteSerializedList(bundle.NamePlates, options.Compressed);
        
        writer.WriteAllSpritesheet(bundle.AllSpritesheet, options.Compressed);
    }

    private static void WriteAllSpritesheet(this BinaryWriter writer, Dictionary<string, byte[]> allSpritesheet, bool isCompressed)
    {
        writer.Write(allSpritesheet.Count);
        foreach (var (fileName, data) in allSpritesheet)
        {
            var nameBytes = Encoding.UTF8.GetBytes(fileName);
            writer.Write(nameBytes.Length);
            writer.Write(nameBytes);

            var dataToWrite = isCompressed ? Compress(data) : data;

            writer.Write(dataToWrite.Length);
            writer.Write(dataToWrite);
        }
    }

    internal static Dictionary<string, byte[]> ReadAllSpritesheet(this BinaryReader reader, bool isCompressed)
    {
        var fileCount = reader.ReadInt32();
        var allSpritesheet = new Dictionary<string, byte[]>();
        
        for (var i = 0; i < fileCount; i++)
        {
            var nameLen = reader.ReadInt32();
            var fileName = Encoding.UTF8.GetString(reader.ReadBytes(nameLen));
            var dataLen = reader.ReadInt32();
            var rawData = reader.ReadBytes(dataLen);

            var data = isCompressed ? Decompress(rawData) : rawData;

            allSpritesheet.Add(fileName, data);
        }
        
        return allSpritesheet;
    }

    private static void WriteSerializedList<T>(this BinaryWriter writer, List<T> list, bool isCompressed)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(list);
        var toWrite = isCompressed ? Compress(bytes) : bytes;
        
        writer.Write(toWrite.Length);
        writer.Write(toWrite);
    }

    internal static List<T> ReadSerializedList<T>(this BinaryReader reader, bool isCompressed)
    {
        var size = reader.ReadInt32();
        var bytes = reader.ReadBytes(size);
        var toRead = isCompressed ? Decompress(bytes) : bytes;
        return JsonSerializer.Deserialize<List<T>>(toRead) ?? [];
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
    
    private static byte[] Compress(byte[] data)
    {
        using var ms = new MemoryStream();
        using (var brotli = new BrotliStream(ms, CompressionLevel.SmallestSize, leaveOpen: true))
            brotli.Write(data, 0, data.Length);
        return ms.ToArray();
    }

    private static byte[] Decompress(byte[] data)
    {
        using var input = new MemoryStream(data);
        using var brotli = new BrotliStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        brotli.CopyTo(output);
        return output.ToArray();
    }
}