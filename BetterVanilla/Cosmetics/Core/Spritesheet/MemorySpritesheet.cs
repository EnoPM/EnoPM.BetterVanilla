using System;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core.Spritesheet;

public sealed class MemorySpritesheet : BaseSpritesheet
{
    private string Name { get; }
    private byte[] Data { get; set; }

    public MemorySpritesheet(string name, byte[] data)
    {
        Name = name;
        Data = data;
    }

    protected override Texture2D OpenSpritesheet()
    {
        var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true)
        {
            name = $"{Name}Texture"
        };

        var loaded = texture.LoadImage(Data, false);
        Data = [];
        
        if (!loaded)
        {
            throw new Exception($"Unable to load texture {Name}");
        }

        texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;

        return texture;
    }

    public override Sprite LoadSprite(SerializedSprite serialized)
    {
        var cacheKey = GetCacheKey(serialized);
        if (Cache.TryGetValue(cacheKey, out var sprite))
        {
            return sprite;
        }

        var rect = new Rect(
            serialized.X,
            Spritesheet.height - serialized.Y - serialized.Height,
            serialized.Width,
            serialized.Height
        );

        sprite = Sprite.Create(Spritesheet, rect, Pivot, PixelsPerUnit);
        sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        sprite.name = "BetterVanillaSprite";

        return Cache[cacheKey] = sprite;
    }

    private static string GetCacheKey(SerializedSprite s) => $"{s.Path};{s.X};{s.Y};{s.Width};{s.Height}";
}