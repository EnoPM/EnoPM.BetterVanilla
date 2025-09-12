using System;
using System.Collections.Generic;
using BetterVanilla.Core;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core.Spritesheet;

public sealed class SpritesheetCache
{
    private Dictionary<string, byte[]> Uncached { get; }
    private Dictionary<string, BaseSpritesheet> Cache { get; } = new();
    
    public SpritesheetCache(Dictionary<string, byte[]> spritesheet)
    {
        Uncached = spritesheet;
    }

    public Sprite GetSprite(SerializedSprite sprite)
    {
        var spritesheet = GetSpritesheet(sprite.Path);

        if (spritesheet == null)
        {
            throw new Exception($"Spritesheet not found for {sprite.Path}");
        }
        
        return spritesheet.LoadSprite(sprite);
    }

    public BaseSpritesheet GetSpritesheet(string name)
    {
        if (!Cache.TryGetValue(name, out var spritesheet))
        {
            Cache[name] = spritesheet = CreateSpritesheet(name);
        }
        return spritesheet;
    }

    private BaseSpritesheet CreateSpritesheet(string name)
    {
        if (!Uncached.Remove(name, out var data))
        {
            throw new KeyNotFoundException($"Spritesheet {name} not found");
        }
        return new MemorySpritesheet(name, data);
    }
}