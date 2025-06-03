using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Utils;

public static class HatUtility
{
    public static readonly string HatsDirectory;
    public static readonly string HatsAnimationsDirectory;
    public static readonly Dictionary<string, HatCosmetic> CustomHatRegistry = new();
    public static readonly Dictionary<string, HatViewData> CustomHatViewDatas = new();
    
    public static Material? HatMaterial { get; set; }

    static HatUtility()
    {
        HatsDirectory = Path.Combine(StorageUtility.CosmeticsDirectory, "Hats");
        if (!Directory.Exists(HatsDirectory))
        {
            Directory.CreateDirectory(HatsDirectory);
        }
        
        HatsAnimationsDirectory = Path.Combine(HatsDirectory, "Animations");
        if (!Directory.Exists(HatsAnimationsDirectory))
        {
            Directory.CreateDirectory(HatsAnimationsDirectory);
        }
    }
    
    public static List<HatCosmeticApi> CreateHatCosmeticApisFromFilenames(string[] filenames, bool fromDisk = false)
    {
        var fronts = new Dictionary<string, HatCosmeticApi>();
        var backs = new Dictionary<string, string>();
        var flips = new Dictionary<string, string>();
        var backFlips = new Dictionary<string, string>();
        var climbs = new Dictionary<string, string>();

        foreach (var filename in filenames)
        {
            var data = new HatCosmeticFiledata(filename, fromDisk);

            if (data.Options.Contains("back") && data.Options.Contains("flip"))
                backFlips[data.Key] = data.FullPath;
            else if (data.Options.Contains("climb"))
                climbs[data.Key] = data.FullPath;
            else if (data.Options.Contains("back"))
                backs[data.Key] = data.FullPath;
            else if (data.Options.Contains("flip"))
                flips[data.Key] = data.FullPath;
            else
            {
                fronts[data.Key] = new HatCosmeticApi
                {
                    Resource = data.FullPath,
                    Name = data.Key.Replace('-', ' '),
                    Bounce = data.Options.Contains("bounce"),
                    Adaptive = data.Options.Contains("adaptive"),
                    Behind = data.Options.Contains("behind")
                };
            }
        }

        var result = new List<HatCosmeticApi>();
        foreach (var (key, hat) in fronts)
        {
            if (backs.TryGetValue(key, out var back)) hat.BackResource = back;
            if (climbs.TryGetValue(key, out var climb)) hat.ClimbResource = climb;
            if (flips.TryGetValue(key, out var flip)) hat.FlipResource = flip;
            if (backFlips.TryGetValue(key, out var backFlip)) hat.BackFlipResource = backFlip;

            if (hat.BackResource != null) hat.Behind = true;

            result.Add(hat);
        }

        return result;
    }
    
    public static Sprite? CreateHatSprite(string path, bool fromDisk = false)
    {
        var texture = fromDisk ? TextureUtility.LoadTextureFromDisk(path) : TextureUtility.LoadTextureFromResources(path);
        if (texture == null)
        {
            return null;
        }
            
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.53f, 0.575f), texture.width * 0.375f);
        if (sprite == null)
        {
            return null;
        }
        
        texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        
        return sprite;
    }

}