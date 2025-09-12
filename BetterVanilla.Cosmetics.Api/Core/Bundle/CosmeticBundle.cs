using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using BetterVanilla.Cosmetics.Api.Hats;
using BetterVanilla.Cosmetics.Api.NamePlates;
using BetterVanilla.Cosmetics.Api.Visors;

namespace BetterVanilla.Cosmetics.Api.Core.Bundle;

public sealed class CosmeticBundle
{
    public Dictionary<string, byte[]> AllSpritesheet { get; internal set; } = new();

    public List<SerializedHat> Hats { get; internal set; } = [];
    public List<SerializedVisor> Visors { get; internal set; } = [];
    
    public List<SerializedNamePlate> NamePlates { get; internal set; } = [];

    private readonly Dictionary<string, string> _spritesheetNamesMap = new();

    public void AddHat(SerializedHat cosmetic)
    {
        CacheSprite(cosmetic.MainResource);
        CacheSprite(cosmetic.PreviewResource);
        CacheSprite(cosmetic.FlipResource);
        CacheSprite(cosmetic.BackResource);
        CacheSprite(cosmetic.BackFlipResource);
        CacheSprite(cosmetic.ClimbResource);
        CacheSprite(cosmetic.FrontAnimationFrames);
        CacheSprite(cosmetic.BackAnimationFrames);

        Hats.Add(cosmetic);
    }

    public void AddVisor(SerializedVisor cosmetic)
    {
        CacheSprite(cosmetic.MainResource);
        CacheSprite(cosmetic.PreviewResource);
        CacheSprite(cosmetic.LeftResource);
        CacheSprite(cosmetic.ClimbResource);
        CacheSprite(cosmetic.FloorResource);
        CacheSprite(cosmetic.FrontAnimationFrames);
        
        Visors.Add(cosmetic);
    }

    public void AddNamePlate(SerializedNamePlate cosmetic)
    {
        CacheSprite(cosmetic.MainResource);
        
        NamePlates.Add(cosmetic);
    }

    private void CacheSprite(List<SerializedSprite>? sprites)
    {
        if (sprites == null) return;
        foreach (var sprite in sprites)
        {
            CacheSprite(sprite);
        }
    }

    private void CacheSprite(SerializedSprite? sprite)
    {
        if (sprite == null) return;
        if (!_spritesheetNamesMap.TryGetValue(sprite.Path, out var cachedName))
        {
            cachedName = CacheSpritesheetFile(sprite.Path);
            _spritesheetNamesMap.Add(sprite.Path, cachedName);
        }
        sprite.Path = cachedName;
    }

    private string CacheSpritesheetFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Sprite file '{filePath}' not found");
        }

        var fileContent = File.ReadAllBytes(filePath);
        var translatedName = Guid.NewGuid().ToString();
        AllSpritesheet.Add(translatedName, fileContent);
        return translatedName;
    }

    public void Serialize(Stream stream, bool compressed = false)
    {
        BundleSerializer.SerializeBundle(this, stream, new BundleSerializerOptions
        {
            Compressed = compressed
        });
    }

    public static CosmeticBundle Deserialize(Stream stream)
    {
        return BundleSerializer.DeserializeBundle(stream);
    }

    public static CosmeticBundle FromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Bundle file not found", filePath);
        }
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return Deserialize(stream);
    }
}