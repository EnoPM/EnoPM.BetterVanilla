using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Cosmetics.Api.Hats;
using BetterVanilla.Cosmetics.Api.Serialization;
using BetterVanilla.Cosmetics.Core.Spritesheet;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Hats;

public sealed class HatCosmetic : IHat<Sprite>, IHatExtension
{
    public string Name { get; set; }

    public bool Adaptive { get; set; }

    public bool Bounce { get; set; }

    public SerializedCosmeticAuthor? Author { get; set; }

    public Sprite MainResource { get; set; }
    
    public Sprite? FlipResource { get; set; }

    public Sprite? BackResource { get; set; }

    public Sprite? BackFlipResource { get; set; }

    public Sprite? ClimbResource { get; set; }

    public List<Sprite>? FrontAnimationFrames { get; set; }

    public List<Sprite>? BackAnimationFrames { get; set; }
    
    public bool Behind { get; set; }
    
    public int CurrentFrontFrame { get; set; }
    
    public int CurrentBackFrame { get; set; }

    public int FrontDelay { get; set; } = 10;
    public int BackDelay { get; set; } = 10;
    
    public float FrontTime { get; set; }
    public float BackTime { get; set; }
    
    public void UpdateFrontFrames()
    {
        if (FrontAnimationFrames != null && FrontAnimationFrames.Count > 0)
        {
            CurrentFrontFrame++;
            if (CurrentFrontFrame >= FrontAnimationFrames.Count)
            {
                CurrentFrontFrame = 0;
            }
        }
    }

    public void UpdateBackFrames()
    {
        if (BackAnimationFrames != null && BackAnimationFrames.Count > 0)
        {
            CurrentBackFrame++;
            if (CurrentBackFrame >= BackAnimationFrames.Count)
            {
                CurrentBackFrame = 0;
            }
        }
    }

    public HatCosmetic(SerializedHat hat, BaseSpritesheet spritesheet) : this(hat, spritesheet.LoadSprite(hat.MainResource))
    {
        BackResource = hat.BackResource == null ? null : spritesheet.LoadSprite(hat.BackResource);
        BackFlipResource = hat.BackFlipResource == null ? null : spritesheet.LoadSprite(hat.BackFlipResource);
        ClimbResource = hat.ClimbResource == null ? null : spritesheet.LoadSprite(hat.ClimbResource);
        FrontAnimationFrames = hat.FrontAnimationFrames?.Select(spritesheet.LoadSprite).ToList();
        BackAnimationFrames = hat.BackAnimationFrames?.Select(spritesheet.LoadSprite).ToList();
    }

    public HatCosmetic(string name, Sprite mainResource)
    {
        Name = name;
        MainResource = mainResource;
    }

    public HatCosmetic(SerializedHat hat, SpritesheetCache cache) : this(hat, cache.GetSprite(hat.MainResource))
    {
        BackResource = hat.BackResource != null ? cache.GetSprite(hat.BackResource) : null;
        BackFlipResource = hat.BackFlipResource != null ? cache.GetSprite(hat.BackFlipResource) : null;
        ClimbResource = hat.ClimbResource != null ? cache.GetSprite(hat.ClimbResource) : null;
        FrontAnimationFrames = hat.FrontAnimationFrames?.Select(cache.GetSprite).ToList();
        BackAnimationFrames = hat.BackAnimationFrames?.Select(cache.GetSprite).ToList();
    }

    private HatCosmetic(SerializedHat hat, Sprite mainResource)
    {
        Adaptive = hat.Adaptive;
        Bounce = hat.Bounce;
        Author = hat.Author;
        Name = hat.Name;
        MainResource = mainResource;
    }
}