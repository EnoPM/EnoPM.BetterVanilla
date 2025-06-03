using System.IO;
using BetterVanilla.Cosmetics.Utils;
using UnityEngine;

namespace BetterVanilla.Cosmetics.Data;

public class HatCosmeticHashes : HatCosmeticApi
{
    public string ResHashA { get; set; }
    public string ResHashB { get; set; }
    public string ResHashC { get; set; }
    public string ResHashF { get; set; }
    public string ResHashBF { get; set; }

    public override HatData CreateCosmeticBehaviour(bool fromDisk = false, bool testOnly = false)
    {
        var filePath = Path.Combine(HatUtility.HatsDirectory, Resource);
        Resource = Path.Combine(filePath, Resource);
        if (BackResource != null)
        {
            BackResource = Path.Combine(filePath, BackResource);
        }
        if (ClimbResource != null)
        {
            ClimbResource = Path.Combine(filePath, ClimbResource);
        }
        if (FlipResource != null)
        {
            FlipResource = Path.Combine(filePath, FlipResource);
        }
        if (BackFlipResource != null)
        {
            BackFlipResource = Path.Combine(filePath, BackFlipResource);
        }
        
        return base.CreateCosmeticBehaviour(fromDisk, testOnly);
    }
}