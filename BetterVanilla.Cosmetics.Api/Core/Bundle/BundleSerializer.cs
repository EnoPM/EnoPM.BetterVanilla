using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using BetterVanilla.Cosmetics.Api.Core.Bundle.Versions;
using BetterVanilla.Cosmetics.Api.Core.Serialization;

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
        
        writer.WriteSerializedList(bundle.Hats, CosmeticsJsonContext.Default.ListSerializedHat, options.Compressed);
        writer.WriteSerializedList(bundle.Visors, CosmeticsJsonContext.Default.ListSerializedVisor, options.Compressed);
        writer.WriteSerializedList(bundle.NamePlates, CosmeticsJsonContext.Default.ListSerializedNamePlate, options.Compressed);
        
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

            var dataToWrite = isCompressed ? ByteCompressor.Compress(data) : data;

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

            var data = isCompressed ? ByteCompressor.Decompress(rawData) : rawData;

            allSpritesheet.Add(fileName, data);
        }
        
        return allSpritesheet;
    }

    private static void WriteSerializedList<T>(this BinaryWriter writer, List<T> list, JsonTypeInfo<List<T>> jsonTypeInfo, bool isCompressed)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(list, jsonTypeInfo);
        var toWrite = isCompressed ? ByteCompressor.Compress(bytes) : bytes;

        writer.Write(toWrite.Length);
        writer.Write(toWrite);
    }

    internal static List<T> ReadSerializedList<T>(this BinaryReader reader, JsonTypeInfo<List<T>> jsonTypeInfo, bool isCompressed)
    {
        var size = reader.ReadInt32();
        var bytes = reader.ReadBytes(size);
        var toRead = isCompressed ? ByteCompressor.Decompress(bytes) : bytes;
        return JsonSerializer.Deserialize(toRead, jsonTypeInfo) ?? [];
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
    
    
}