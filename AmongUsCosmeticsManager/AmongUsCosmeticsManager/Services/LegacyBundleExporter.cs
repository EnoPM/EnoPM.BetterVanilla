using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using AmongUsCosmeticsManager.Models;
using AmongUsCosmeticsManager.Services.BundleImport;
using BetterVanilla.Cosmetics.Serialization;

namespace AmongUsCosmeticsManager.Services;

/// <summary>
/// Exports a CosmeticBundle in the legacy binary format (.bundle)
/// compatible with BetterVanilla.Cosmetics.Api / CosmeticsCompiler.
/// Format: [int32 version=1] [bool compressed] [hats json] [visors json] [nameplates json] [spritesheets]
/// </summary>
public static class LegacyBundleExporter
{
    private const int Version = 1;

    public static byte[] Export(CosmeticBundle bundle, bool compressed = true)
    {
        // Build a SerializableBundle to get the spritesheet deduplication
        var sb = BundleCompileService.BuildSerializableBundle(bundle);

        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);

        writer.Write(Version);
        writer.Write(compressed);

        // Write JSON lists using the same property names as the legacy API
        WriteJsonList(writer, sb.Hats, LegacyJsonContext.Default.ListSerializableHat, compressed);
        WriteJsonList(writer, sb.Visors, LegacyJsonContext.Default.ListSerializableVisor, compressed);
        WriteJsonList(writer, sb.Nameplates, LegacyJsonContext.Default.ListSerializableNameplate, compressed);

        // Write spritesheets
        writer.Write(sb.AllSpritesheet.Count);
        foreach (var (name, data) in sb.AllSpritesheet)
        {
            var nameBytes = Encoding.UTF8.GetBytes(name);
            writer.Write(nameBytes.Length);
            writer.Write(nameBytes);

            var dataToWrite = compressed ? Compress(data) : data;
            writer.Write(dataToWrite.Length);
            writer.Write(dataToWrite);
        }

        writer.Flush();
        return ms.ToArray();
    }

    private static void WriteJsonList<T>(BinaryWriter writer, List<T> list, System.Text.Json.Serialization.Metadata.JsonTypeInfo<List<T>> jsonTypeInfo, bool compressed)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(list, jsonTypeInfo);
        var toWrite = compressed ? Compress(bytes) : bytes;
        writer.Write(toWrite.Length);
        writer.Write(toWrite);
    }

    private static byte[] Compress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(output, CompressionLevel.SmallestSize))
        {
            brotli.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }
}
