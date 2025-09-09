using System.Collections.Generic;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core.Spritesheet;

public abstract class BaseSpritesheet
{
    protected const float PixelsPerUnit = 115f;
    protected static Vector2 Pivot { get; } = new(0.53f, 0.575f);
    
    private Texture2D? CachedSpritesheet { get; set; }
    protected Dictionary<string, Sprite> Cache { get; } = [];
    protected Texture2D Spritesheet
    {
        get
        {
            if (CachedSpritesheet == null)
            {
                CachedSpritesheet = OpenSpritesheet();
            }
            return CachedSpritesheet;
        }
    }

    protected abstract Texture2D OpenSpritesheet();

    public abstract Sprite LoadSprite(SerializedSprite serialized);
}