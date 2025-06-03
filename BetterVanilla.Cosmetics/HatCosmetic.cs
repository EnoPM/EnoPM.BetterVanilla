using System.Collections.Generic;
using UnityEngine;

namespace BetterVanilla.Cosmetics;

public sealed class HatCosmetic
{
    public string Author { get; set; }
    public string Package { get; set; }
    public Sprite? FlipImage { get; set; }
    public Sprite? BackFlipImage { get; set; }
    public List<Sprite?>? Animation { get; set; }
    public List<Sprite> BackAnimation { get; set; }
    public int Frame { get; set; }
    public float Time { get; set; }
}