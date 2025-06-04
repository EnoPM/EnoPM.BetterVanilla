﻿using UnityEngine;

namespace BetterVanilla.Cosmetics.Data;

public class HatExtension
{
    public string Author { get; set; }
    public string Package { get; set; }
    public string Condition { get; set; }
    public Sprite? FlipImage { get; set; }
    public Sprite? BackFlipImage { get; set; }
}