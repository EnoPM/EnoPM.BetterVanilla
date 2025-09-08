using System;
using System.IO;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core.Spritesheet;

public sealed class FileSpritesheet : BaseSpritesheet
{
    private string FilePath { get; }

    public FileSpritesheet(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Spritesheet file not found", filePath);
        }
        FilePath = filePath;
    }

    protected override Texture2D OpenSpritesheet()
    {
        var textureName = Path.GetFileNameWithoutExtension(FilePath);
        var data = File.ReadAllBytes(FilePath);
        var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true)
        {
            name = $"{textureName}Texture"
        };

        if (!texture.LoadImage(data, false))
        {
            throw new Exception($"Unable to load texture {FilePath}");
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
        var pivot = new Vector2(0.53f, 0.575f);

        sprite = Sprite.Create(Spritesheet, rect, pivot, 100f);
        sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;

        return Cache[cacheKey] = sprite;
    }

    private static string GetCacheKey(SerializedSprite s) => $"{s.Path};{s.X};{s.Y};{s.Width};{s.Height}";
}