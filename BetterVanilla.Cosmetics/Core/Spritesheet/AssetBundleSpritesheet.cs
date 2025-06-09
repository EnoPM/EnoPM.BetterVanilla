using System;
using BetterVanilla.Cosmetics.Api.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Core.Spritesheet;

public sealed class AssetBundleSpritesheet : BaseSpritesheet
{
    protected override Texture2D OpenSpritesheet()
    {
        throw new NotImplementedException();
    }
    public override Sprite LoadSprite(SerializedSprite serialized)
    {
        throw new NotImplementedException();
    }
}