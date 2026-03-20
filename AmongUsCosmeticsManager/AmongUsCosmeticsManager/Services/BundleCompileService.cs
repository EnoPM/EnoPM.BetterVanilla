using System;
using System.Collections.Generic;
using System.Linq;
using AmongUsCosmeticsManager.Models;
using AmongUsCosmeticsManager.Models.Animation;
using BetterVanilla.Cosmetics.Serialization;
using SkiaSharp;

namespace AmongUsCosmeticsManager.Services;

public static class BundleCompileService
{
    public static byte[] Compile(CosmeticBundle bundle)
    {
        var sb = BuildSerializableBundle(bundle);
        var raw = AppSerializerContext.Instance.Serialize(sb);
        return Compress(raw);
    }

    private static byte[] Compress(byte[] data)
    {
        using var output = new System.IO.MemoryStream();
        using (var brotli = new System.IO.Compression.BrotliStream(output, System.IO.Compression.CompressionLevel.SmallestSize))
        {
            brotli.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    public static SerializableBundle BuildSerializableBundle(CosmeticBundle bundle)
    {
        var sb = new SerializableBundle();

        var hatSection = bundle.GetSection("hat");
        if (hatSection != null)
        {
            foreach (var item in hatSection.Items)
                sb.AddHat(MapHat(item));
        }

        var visorSection = bundle.GetSection("visor");
        if (visorSection != null)
        {
            foreach (var item in visorSection.Items)
                sb.AddVisor(MapVisor(item));
        }

        var nameplateSection = bundle.GetSection("nameplate");
        if (nameplateSection != null)
        {
            foreach (var item in nameplateSection.Items)
                sb.AddNameplate(MapNameplate(item));
        }

        return sb;
    }

    private static SerializableHat MapHat(CosmeticItem item)
    {
        return new SerializableHat
        {
            Name = item.Name,
            IsAdaptive = item.GetProperty("adaptive")?.BoolValue ?? false,
            Author = MapAuthor(item.Author),
            IsBounce = item.GetProperty("bounce")?.BoolValue ?? false,
            DisableVisors = item.GetProperty("noVisors")?.BoolValue ?? false,
            Front = MapSprite(item.GetResource("front")),
            FrontAnimation = MapAnimation(item, "frontAnimation"),
            Flip = MapSprite(item.GetResource("flip")),
            FlipAnimation = MapAnimation(item, "flipAnimation"),
            Back = MapSprite(item.GetResource("back")),
            BackAnimation = MapAnimation(item, "backAnimation"),
            BackFlip = MapSprite(item.GetResource("backFlip")),
            BackFlipAnimation = MapAnimation(item, "backFlipAnimation"),
            Climb = MapSprite(item.GetResource("climb")),
            ClimbAnimation = MapAnimation(item, "climbAnimation"),
            Preview = MapPreview(item, item.GetResource("front")),
            PreviewAnimation = MapPreviewAnimation(item),
        };
    }

    private static SerializableVisor MapVisor(CosmeticItem item)
    {
        return new SerializableVisor
        {
            Name = item.Name,
            IsAdaptive = item.GetProperty("adaptive")?.BoolValue ?? false,
            Author = MapAuthor(item.Author),
            IsBehindHat = item.GetProperty("behindHats")?.BoolValue ?? false,
            Front = MapSprite(item.GetResource("front")),
            FrontAnimation = MapAnimation(item, "frontAnimation"),
            Left = MapSprite(item.GetResource("left")),
            LeftAnimation = MapAnimation(item, "leftAnimation"),
            Floor = MapSprite(item.GetResource("floor")),
            FloorAnimation = MapAnimation(item, "floorAnimation"),
            Preview = MapPreview(item, item.GetResource("front")),
            PreviewAnimation = MapPreviewAnimation(item),
        };
    }

    private static SerializableNameplate MapNameplate(CosmeticItem item)
    {
        return new SerializableNameplate
        {
            Name = item.Name,
            IsAdaptive = item.GetProperty("adaptive")?.BoolValue ?? false,
            Author = MapAuthor(item.Author),
            Resource = MapSprite(item.GetResource("resource")),
            ResourceAnimation = MapAnimation(item, "resourceAnimation"),
            Preview = MapPreview(item, item.GetResource("resource")),
            PreviewAnimation = MapPreviewAnimation(item),
        };
    }

    private static SerializableAuthor? MapAuthor(string? author)
    {
        return string.IsNullOrWhiteSpace(author) ? null : new SerializableAuthor { Name = author };
    }

    private static SerializableSprite? MapPreview(CosmeticItem item, ResourceValue? fallbackResource)
    {
        var previewResource = item.GetResource("preview");
        if (previewResource != null && previewResource.HasData)
            return MapSprite(previewResource);

        return MapSprite(fallbackResource);
    }

    private static SerializableFrameAnimation? MapPreviewAnimation(CosmeticItem item)
    {
        return MapAnimation(item, "previewAnimation");
    }

    private static SerializableSprite? MapSprite(ResourceValue? resource)
    {
        if (resource == null || resource.Data == null || resource.Data.Length == 0)
            return null;

        var (w, h) = GetImageDimensions(resource.Data);

        return new SerializableSprite
        {
            Data = resource.Data,
            X = 0,
            Y = 0,
            Width = w,
            Height = h
        };
    }

    private static SerializableFrameAnimation? MapAnimation(CosmeticItem item, string frameListId)
    {
        var fl = item.FrameLists.FirstOrDefault(f => f.Definition.Id == frameListId);
        if (fl == null || fl.Nodes.Count == 0) return null;

        var defaultDuration = 1000 / Math.Max(1, fl.DefaultFps);
        var sprites = new List<SerializableSprite>();
        FlattenNodes(fl.Nodes, sprites, defaultDuration);
        if (sprites.Count == 0) return null;

        return new SerializableFrameAnimation
        {
            Fps = fl.DefaultFps,
            Frames = sprites.ToArray()
        };
    }

    private static void FlattenNodes(IEnumerable<AnimationNode> nodes, List<SerializableSprite> output, int defaultDuration)
    {
        foreach (var node in nodes)
        {
            switch (node)
            {
                case FrameNode frame when frame.Data.Length > 0:
                    var (w, h) = GetImageDimensions(frame.Data);
                    output.Add(new SerializableSprite
                    {
                        Data = frame.Data, X = 0, Y = 0, Width = w, Height = h,
                        DurationMs = frame.DurationMs ?? defaultDuration
                    });
                    break;
                case DelayNode delay:
                    // Emit a null-data sprite with the delay duration
                    output.Add(new SerializableSprite
                    {
                        DurationMs = delay.DurationMs
                    });
                    break;
                case LoopNode loop:
                    var loopSprites = new List<SerializableSprite>();
                    FlattenNodes(loop.Children, loopSprites, defaultDuration);
                    for (var i = 0; i < Math.Max(1, loop.Count); i++)
                        output.AddRange(loopSprites);
                    break;
            }
        }
    }

    private static (int Width, int Height) GetImageDimensions(byte[] data)
    {
        using var codec = SKCodec.Create(new SKMemoryStream(data));
        if (codec == null) return (0, 0);
        return (codec.Info.Width, codec.Info.Height);
    }
}
