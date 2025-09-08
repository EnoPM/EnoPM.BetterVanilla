using System.Collections.Generic;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core.Spritesheet;

public abstract class BaseSpritesheet
{
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