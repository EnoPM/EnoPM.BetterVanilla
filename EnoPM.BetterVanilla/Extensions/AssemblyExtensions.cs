using System;
using System.Collections.Generic;
using System.Reflection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace EnoPM.BetterVanilla.Extensions;

public static class AssemblyExtensions
{
    private static readonly Dictionary<string, Sprite> SpritesCache = new();
    private static readonly Dictionary<string, Sprite> AutoSizedSpritesCache = new();
    
    public static Sprite LoadSpriteFromResources(this Assembly assembly, string path, float pixelsPerUnit)
    {
        var cacheKey = $"{path}{pixelsPerUnit}";
        if (SpritesCache.TryGetValue(cacheKey, out var sprite)) return sprite;
        var texture = assembly.LoadTextureFromResources(path);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
        return SpritesCache[cacheKey] = sprite;
    }
    
    public static Sprite LoadSpriteFromResources(this Assembly assembly, string path)
    {
        if (AutoSizedSpritesCache.TryGetValue(path, out var sprite)) return sprite;
        var texture = assembly.LoadTextureFromResources(path);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
        sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
        return AutoSizedSpritesCache[path] = sprite;
    }
    
    private static unsafe Texture2D LoadTextureFromResources(this Assembly assembly, string path)
    {
        var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        var stream = assembly.GetManifestResourceStream(path);
        if (stream == null) return null;
        var length = stream.Length;
        var bytes = new Il2CppStructArray<byte>(length);
        _ = stream.Read(new Span<byte>(IntPtr.Add(bytes.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
        ImageConversion.LoadImage(texture, bytes, false);
        return texture;
    }
}