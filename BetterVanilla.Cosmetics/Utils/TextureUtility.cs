using System;
using System.IO;
using System.Reflection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Utils;

public static class TextureUtility
{
    public static Texture2D? LoadTextureFromDisk(string path)
    {
        if (!File.Exists(path))
        {
            CosmeticsPlugin.Logging.LogError($"Unable to find texture file: {path}");
            return null;
        }
        
        var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        
        try
        {
            texture.LoadImage(File.ReadAllBytes(path), false);
            return texture;
        }
        catch (Exception ex)
        {
            CosmeticsPlugin.Logging.LogError($"Unable to load texture from disk: {path} - {ex.StackTrace}");
            return null;
        }
    }
    
    public static unsafe Texture2D? LoadTextureFromResources(string path)
    {
        try
        {
            Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
            var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(path);
            if (stream == null)
            {
                CosmeticsPlugin.Logging.LogError($"Unable to find texture resource: {path}");
                return null;
            }
            var length = stream.Length;
            var byteTexture = new Il2CppStructArray<byte>(length);
            _ = stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
            texture.LoadImage(byteTexture, false);
            return texture;
        }
        catch (Exception ex)
        {
            CosmeticsPlugin.Logging.LogError($"Error loading texture from resources: {path} - {ex.StackTrace}");
            return null;
        }
    }
}