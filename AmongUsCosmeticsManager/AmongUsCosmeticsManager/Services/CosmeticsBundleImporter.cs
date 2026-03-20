using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using AmongUsCosmeticsManager.Models;
using AmongUsCosmeticsManager.Models.Animation;
using AmongUsCosmeticsManager.Models.Config;
using BetterVanilla.Cosmetics.Serialization;
using BetterVanilla.Cosmetics.Serialization.Core;
using SkiaSharp;

namespace AmongUsCosmeticsManager.Services;

/// <summary>
/// Imports a bundle serialized with BSerializer (SerializableBundle format).
/// </summary>
public static class CosmeticsBundleImporter
{
    public static CosmeticBundle Import(string filePath, IProgress<string>? progress = null)
    {
        progress?.Report("Lecture du fichier...");
        var compressed = File.ReadAllBytes(filePath);

        progress?.Report("Décompression...");
        var data = Decompress(compressed);

        progress?.Report("Désérialisation...");
        var sb = AppSerializerContext.Instance.Deserialize<SerializableBundle>(data);

        var bundle = new CosmeticBundle(CosmeticTypeDefinition.All)
        {
            Name = Path.GetFileNameWithoutExtension(filePath)
        };

        var hatSection = bundle.GetSection("hat");
        if (hatSection != null)
        {
            for (var i = 0; i < sb.Hats.Count; i++)
            {
                var hat = sb.Hats[i];
                progress?.Report($"Import hat {i + 1}/{sb.Hats.Count}: {hat.Name}");
                var item = CreateItem(hatSection, hat);
                SetBool(item, "bounce", hat.IsBounce);
                SetBool(item, "noVisors", hat.DisableVisors);
                SetSpriteResource(item, "front", hat.Front, sb);
                SetSpriteResource(item, "flip", hat.Flip, sb);
                SetSpriteResource(item, "back", hat.Back, sb);
                SetSpriteResource(item, "backFlip", hat.BackFlip, sb);
                SetSpriteResource(item, "climb", hat.Climb, sb);
                SetAnimation(item, "frontAnimation", hat.FrontAnimation, sb);
                SetAnimation(item, "flipAnimation", hat.FlipAnimation, sb);
                SetAnimation(item, "backAnimation", hat.BackAnimation, sb);
                SetAnimation(item, "backFlipAnimation", hat.BackFlipAnimation, sb);
                SetAnimation(item, "climbAnimation", hat.ClimbAnimation, sb);
                SetSpriteResource(item, "preview", hat.Preview, sb);
                SetAnimation(item, "previewAnimation", hat.PreviewAnimation, sb);
                hatSection.Items.Add(item);
            }
        }

        var visorSection = bundle.GetSection("visor");
        if (visorSection != null)
        {
            for (var i = 0; i < sb.Visors.Count; i++)
            {
                var visor = sb.Visors[i];
                progress?.Report($"Import visor {i + 1}/{sb.Visors.Count}: {visor.Name}");
                var item = CreateItem(visorSection, visor);
                SetBool(item, "behindHats", visor.IsBehindHat);
                SetSpriteResource(item, "front", visor.Front, sb);
                SetSpriteResource(item, "left", visor.Left, sb);
                SetSpriteResource(item, "floor", visor.Floor, sb);
                SetAnimation(item, "frontAnimation", visor.FrontAnimation, sb);
                SetAnimation(item, "leftAnimation", visor.LeftAnimation, sb);
                SetAnimation(item, "floorAnimation", visor.FloorAnimation, sb);
                SetSpriteResource(item, "preview", visor.Preview, sb);
                SetAnimation(item, "previewAnimation", visor.PreviewAnimation, sb);
                visorSection.Items.Add(item);
            }
        }

        var npSection = bundle.GetSection("nameplate");
        if (npSection != null)
        {
            for (var i = 0; i < sb.Nameplates.Count; i++)
            {
                var np = sb.Nameplates[i];
                progress?.Report($"Import nameplate {i + 1}/{sb.Nameplates.Count}: {np.Name}");
                var item = CreateItem(npSection, np);
                SetSpriteResource(item, "resource", np.Resource, sb);
                SetAnimation(item, "resourceAnimation", np.ResourceAnimation, sb);
                SetSpriteResource(item, "preview", np.Preview, sb);
                SetAnimation(item, "previewAnimation", np.PreviewAnimation, sb);
                npSection.Items.Add(item);
            }
        }

        return bundle;
    }

    private static CosmeticItem CreateItem(CosmeticSection section, SerializableCosmeticBase cosmetic)
    {
        var item = new CosmeticItem(section.TypeDefinition)
        {
            Name = cosmetic.Name,
            Author = cosmetic.Author?.Name ?? string.Empty,
            LastModified = DateTime.Now
        };
        SetBool(item, "adaptive", cosmetic.IsAdaptive);
        return item;
    }

    private static void SetBool(CosmeticItem item, string propId, bool value)
    {
        var pv = item.GetProperty(propId);
        if (pv != null) pv.BoolValue = value;
    }

    private static void SetSpriteResource(CosmeticItem item, string slotId, SerializableSprite? sprite, SerializableBundle sb)
    {
        if (sprite == null) return;
        var rv = item.GetResource(slotId);
        if (rv == null) return;

        var imageData = ResolveSpriteData(sprite, sb);
        if (imageData != null)
        {
            rv.FileName = $"{item.Name}_{slotId}.png";
            rv.Data = imageData;
        }
    }

    private static void SetAnimation(CosmeticItem item, string frameListId, SerializableFrameAnimation? animation, SerializableBundle sb)
    {
        if (animation == null || animation.Steps.Length == 0) return;
        var fl = item.FrameLists.FirstOrDefault(f => f.Definition.Id == frameListId);
        if (fl == null) return;

        if (animation.Fps > 0) fl.DefaultFps = animation.Fps;

        foreach (var step in animation.Steps)
        {
            switch (step.Type)
            {
                case 0: // Frame
                    if (step.Sprite == null) continue;
                    var data = ResolveSpriteData(step.Sprite, sb);
                    if (data != null)
                    {
                        var node = new FrameNode { Data = data };
                        if (step.DurationMs is > 0)
                            node.DurationMs = step.DurationMs.Value;
                        fl.Nodes.Add(node);
                    }
                    break;
                case 1: // Delay
                    if (step.DurationMs.HasValue)
                        fl.Nodes.Add(new DelayNode { DurationMs = step.DurationMs.Value });
                    break;
            }
        }
    }

    private static byte[]? ResolveSpriteData(SerializableSprite sprite, SerializableBundle sb)
    {
        // If sprite has inline data, use it directly
        if (sprite.Data is { Length: > 0 })
            return CropIfNeeded(sprite.Data, sprite);

        // Otherwise resolve from spritesheet by path
        if (!string.IsNullOrEmpty(sprite.Path) && sb.AllSpritesheet.TryGetValue(sprite.Path, out var sheetData))
            return CropIfNeeded(sheetData, sprite);

        return null;
    }

    private static byte[]? CropIfNeeded(byte[] imageData, SerializableSprite sprite)
    {
        // If X=0, Y=0 and dimensions match the full image, return as-is
        if (sprite.X == 0 && sprite.Y == 0 && sprite.Width == 0 && sprite.Height == 0)
            return imageData;

        using var bitmap = SKBitmap.Decode(imageData);
        if (bitmap == null) return null;

        if (sprite.X == 0 && sprite.Y == 0 && sprite.Width >= bitmap.Width && sprite.Height >= bitmap.Height)
            return imageData;

        var x = Math.Max(0, sprite.X);
        var y = Math.Max(0, sprite.Y);
        var w = Math.Min(sprite.Width > 0 ? sprite.Width : bitmap.Width, bitmap.Width - x);
        var h = Math.Min(sprite.Height > 0 ? sprite.Height : bitmap.Height, bitmap.Height - y);

        if (w <= 0 || h <= 0) return null;

        var rect = new SKRectI(x, y, x + w, y + h);
        using var cropped = new SKBitmap(w, h);
        if (!bitmap.ExtractSubset(cropped, rect)) return null;

        using var image = SKImage.FromBitmap(cropped);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
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
