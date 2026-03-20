using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AmongUsCosmeticsManager.Models;
using AmongUsCosmeticsManager.Models.Animation;
using AmongUsCosmeticsManager.Models.Config;
using SkiaSharp;

namespace AmongUsCosmeticsManager.Services.BundleImport;

public static class BundleImporter
{
    public static CosmeticBundle Import(string filePath, IProgress<string>? progress = null)
    {
        progress?.Report("Lecture du fichier...");

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: false);

        var version = reader.ReadInt32();
        if (version != 1)
            throw new NotSupportedException($"Bundle version {version} is not supported. Expected version 1 (legacy format). File may be corrupted or in a different format.");

        var compressed = reader.ReadBoolean();

        progress?.Report("Décompression des données...");
        var hats = ReadList(reader, ImportJsonContext.Default.ListImportHat, compressed);
        var visors = ReadList(reader, ImportJsonContext.Default.ListImportVisor, compressed);
        var namePlates = ReadList(reader, ImportJsonContext.Default.ListImportNamePlate, compressed);

        progress?.Report("Chargement des spritesheets...");
        var spritesheets = ReadSpritesheets(reader, compressed);

        var bundle = new CosmeticBundle(CosmeticTypeDefinition.All)
        {
            Name = Path.GetFileNameWithoutExtension(filePath)
        };

        var hatSection = bundle.GetSection("hat");
        if (hatSection != null)
        {
            for (var hi = 0; hi < hats.Count; hi++)
            {
                var hat = hats[hi];
                progress?.Report($"Import hat {hi + 1}/{hats.Count}: {hat.Name}");
                var item = new CosmeticItem(hatSection.TypeDefinition)
                {
                    Name = hat.Name,
                    Author = hat.Author?.Name ?? string.Empty,
                    LastModified = DateTime.Now
                };

                SetBool(item, "adaptive", hat.Adaptive);
                SetBool(item, "bounce", hat.Bounce);
                SetBool(item, "noVisors", hat.NoVisors);

                SetResource(item, "front", hat.MainResource, spritesheets);
                SetResource(item, "flip", hat.FlipResource, spritesheets);
                SetResource(item, "back", hat.BackResource, spritesheets);
                SetResource(item, "climb", hat.ClimbResource, spritesheets);

                SetFrames(item, "frontAnimation", hat.FrontAnimationFrames, spritesheets);
                SetFrames(item, "backAnimation", hat.BackAnimationFrames, spritesheets);

                hatSection.Items.Add(item);
            }
        }

        var visorSection = bundle.GetSection("visor");
        if (visorSection != null)
        {
            for (var vi = 0; vi < visors.Count; vi++)
            {
                var visor = visors[vi];
                progress?.Report($"Import visor {vi + 1}/{visors.Count}: {visor.Name}");
                var item = new CosmeticItem(visorSection.TypeDefinition)
                {
                    Name = visor.Name,
                    Author = visor.Author?.Name ?? string.Empty,
                    LastModified = DateTime.Now
                };

                SetBool(item, "adaptive", visor.Adaptive);
                SetBool(item, "behindHats", visor.BehindHats);

                SetResource(item, "front", visor.MainResource, spritesheets);
                SetResource(item, "left", visor.ClimbResource, spritesheets);
                SetResource(item, "floor", visor.FloorResource, spritesheets);

                SetFrames(item, "frontAnimation", visor.FrontAnimationFrames, spritesheets);

                visorSection.Items.Add(item);
            }
        }

        var npSection = bundle.GetSection("nameplate");
        if (npSection != null)
        {
            for (var ni = 0; ni < namePlates.Count; ni++)
            {
                var np = namePlates[ni];
                progress?.Report($"Import nameplate {ni + 1}/{namePlates.Count}: {np.Name}");
                var item = new CosmeticItem(npSection.TypeDefinition)
                {
                    Name = np.Name,
                    Author = np.Author?.Name ?? string.Empty,
                    LastModified = DateTime.Now
                };

                SetBool(item, "adaptive", np.Adaptive);
                SetResource(item, "resource", np.MainResource, spritesheets);

                npSection.Items.Add(item);
            }
        }

        return bundle;
    }

    private static void SetBool(CosmeticItem item, string propId, bool value)
    {
        var pv = item.GetProperty(propId);
        if (pv != null) pv.BoolValue = value;
    }

    private static void SetResource(CosmeticItem item, string slotId, ImportSprite? sprite, Dictionary<string, byte[]> spritesheets)
    {
        if (sprite == null) return;
        var rv = item.GetResource(slotId);
        if (rv == null) return;

        var imageData = CropSprite(sprite, spritesheets);
        if (imageData != null)
        {
            rv.FileName = $"{item.Name}_{slotId}.png";
            rv.Data = imageData;
        }
    }

    private const int DefaultImportFps = 10;

    private static void SetFrames(CosmeticItem item, string frameListId, List<ImportSprite>? sprites, Dictionary<string, byte[]> spritesheets)
    {
        if (sprites == null || sprites.Count == 0) return;
        var fl = item.FrameLists[0];
        foreach (var f in item.FrameLists)
        {
            if (f.Definition.Id == frameListId) { fl = f; break; }
        }

        var baseDuration = 1000 / DefaultImportFps;

        // Crop all sprites, caching by (path, x, y, w, h) to avoid re-cropping duplicates
        var cropCache = new Dictionary<(string, int, int, int, int), byte[]?>();
        var cropped = new List<(ImportSprite Sprite, byte[]? Data)>();
        foreach (var sprite in sprites)
        {
            var key = (sprite.Path, sprite.X, sprite.Y, sprite.Width, sprite.Height);
            if (!cropCache.TryGetValue(key, out var data))
            {
                data = CropSprite(sprite, spritesheets);
                cropCache[key] = data;
            }
            cropped.Add((sprite, data));
        }

        // Group consecutive identical sprites into single FrameNodes with duration
        var i = 0;
        while (i < cropped.Count)
        {
            var (sprite, data) = cropped[i];
            if (data == null) { i++; continue; }

            // Count consecutive identical sprites
            var runLength = 1;
            while (i + runLength < cropped.Count && IsSameSprite(sprite, cropped[i + runLength].Sprite))
                runLength++;

            var node = new FrameNode { Data = data };
            if (runLength > 1)
                node.DurationMs = runLength * baseDuration;

            fl.Nodes.Add(node);
            i += runLength;
        }
    }

    private static bool IsSameSprite(ImportSprite a, ImportSprite b)
        => a.Path == b.Path && a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;

    private static byte[]? CropSprite(ImportSprite sprite, Dictionary<string, byte[]> spritesheets)
    {
        if (!spritesheets.TryGetValue(sprite.Path, out var sheetData))
            return null;

        using var bitmap = SKBitmap.Decode(sheetData);
        if (bitmap == null) return null;

        var x = Math.Max(0, sprite.X);
        var y = Math.Max(0, sprite.Y);
        var w = Math.Min(sprite.Width, bitmap.Width - x);
        var h = Math.Min(sprite.Height, bitmap.Height - y);

        if (w <= 0 || h <= 0) return null;

        var rect = new SKRectI(x, y, x + w, y + h);
        using var cropped = new SKBitmap(w, h);
        if (!bitmap.ExtractSubset(cropped, rect))
            return null;

        using var image = SKImage.FromBitmap(cropped);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private static List<T> ReadList<T>(BinaryReader reader, JsonTypeInfo<List<T>> jsonTypeInfo, bool compressed)
    {
        var size = reader.ReadInt32();
        var bytes = reader.ReadBytes(size);
        var toRead = compressed ? Decompress(bytes) : bytes;
        return JsonSerializer.Deserialize(toRead, jsonTypeInfo) ?? [];
    }

    private static Dictionary<string, byte[]> ReadSpritesheets(BinaryReader reader, bool compressed)
    {
        var count = reader.ReadInt32();
        var result = new Dictionary<string, byte[]>();

        for (var i = 0; i < count; i++)
        {
            var nameLen = reader.ReadInt32();
            var name = Encoding.UTF8.GetString(reader.ReadBytes(nameLen));
            var dataLen = reader.ReadInt32();
            var rawData = reader.ReadBytes(dataLen);
            var data = compressed ? Decompress(rawData) : rawData;
            result[name] = data;
        }

        return result;
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
