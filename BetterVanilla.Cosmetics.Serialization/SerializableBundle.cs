using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace BetterVanilla.Cosmetics.Serialization;

public sealed class SerializableBundle
{
    private readonly Dictionary<string, string> _contentHashCache = new();

    public Dictionary<string, byte[]> AllSpritesheet { get; set; } = new();

    public List<SerializableHat> Hats { get; set; } = [];
    public List<SerializableVisor> Visors { get; set; } = [];
    public List<SerializableNameplate> Nameplates { get; set; } = [];

    public void AddHat(SerializableHat cosmetic)
    {
        CacheSprite(cosmetic.Front);
        CacheSprite(cosmetic.FrontAnimation);

        CacheSprite(cosmetic.Flip);
        CacheSprite(cosmetic.FlipAnimation);

        CacheSprite(cosmetic.Back);
        CacheSprite(cosmetic.BackAnimation);

        CacheSprite(cosmetic.BackFlip);
        CacheSprite(cosmetic.BackFlipAnimation);

        CacheSprite(cosmetic.Climb);
        CacheSprite(cosmetic.ClimbAnimation);

        Hats.Add(cosmetic);
    }

    public void AddVisor(SerializableVisor cosmetic)
    {
        CacheSprite(cosmetic.Front);
        CacheSprite(cosmetic.FrontAnimation);

        CacheSprite(cosmetic.Left);
        CacheSprite(cosmetic.LeftAnimation);

        CacheSprite(cosmetic.Floor);
        CacheSprite(cosmetic.FloorAnimation);

        Visors.Add(cosmetic);
    }

    public void AddNameplate(SerializableNameplate cosmetic)
    {
        CacheSprite(cosmetic.Resource);
        CacheSprite(cosmetic.ResourceAnimation);

        Nameplates.Add(cosmetic);
    }

    private void CacheSprite(SerializableFrameAnimation? animation)
    {
        if (animation == null) return;
        foreach (var sprite in animation.Frames)
        {
            CacheSprite(sprite);
        }
    }

    private void CacheSprite(SerializableSprite? sprite)
    {
        if (sprite == null) return;

        var data = sprite.Data ?? LoadFileData(sprite.Path);
        if (data == null || data.Length == 0) return;

        var contentHash = ComputeHash(data);

        if (!_contentHashCache.TryGetValue(contentHash, out var cachedName))
        {
            cachedName = Guid.NewGuid().ToString();
            AllSpritesheet.Add(cachedName, data);
            _contentHashCache.Add(contentHash, cachedName);
        }

        sprite.Path = cachedName;
        sprite.Data = null;
    }

    private static byte[]? LoadFileData(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
        return File.ReadAllBytes(path);
    }

    private static string ComputeHash(byte[] data)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(data);
        return Convert.ToBase64String(hash);
    }
}
