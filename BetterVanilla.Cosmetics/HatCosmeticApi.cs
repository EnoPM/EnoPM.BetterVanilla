using System.Collections.Generic;
using System.IO;
using System.Linq;
using BetterVanilla.Cosmetics.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BetterVanilla.Cosmetics;

public class HatCosmeticApi
{
    public string? Author { get; set; }
    public string? Package { get; set; }
    public string Condition { get; set; }
    public string Name { get; set; }
    public string Resource { get; set; }
    public string? FlipResource { get; set; }
    public string? BackFlipResource { get; set; }
    public string? BackResource { get; set; }
    public string? ClimbResource { get; set; }
    public List<string>? Animation { get; set; }
    public string AnimationPrefix { get; set; }
    public List<string> BackAnimation { get; set; }
    public string BackAnimationPrefix { get; set; }
    public bool Bounce { get; set; }
    public bool Adaptive { get; set; }
    public bool Behind { get; set; }

    public virtual HatData CreateCosmeticBehaviour(bool fromDisk = false, bool testOnly = false)
    {
        if (!HatUtility.HatMaterial)
        {
            HatUtility.HatMaterial = HatManager.Instance.PlayerMaterial;
        }

        var viewData = ScriptableObject.CreateInstance<HatViewData>();
        viewData.FloorImage = viewData.MainImage = HatUtility.CreateHatSprite(Resource, fromDisk);
        if (BackResource != null)
        {
            viewData.BackImage = HatUtility.CreateHatSprite(BackResource, fromDisk);
            Behind = true;
        }

        if (ClimbResource != null)
        {
            viewData.LeftClimbImage = viewData.ClimbImage = HatUtility.CreateHatSprite(ClimbResource, fromDisk);
        }

        var hat = ScriptableObject.CreateInstance<HatData>();

        hat.name = Name;
        hat.displayOrder = 99;
        hat.ProductId = "hat_" + Name.Replace(' ', '_');
        hat.InFront = !Behind;
        hat.ChipOffset = new Vector2(0f, 0.2f);
        hat.Free = true;

        if (Adaptive)
        {
            viewData.MatchPlayerColor = true;
        }

        var cosmetic = new HatCosmetic
        {
            Author = Author ?? "Unknown",
            Package = Package ?? "Misc."
        };

        if (FlipResource != null)
        {
            cosmetic.FlipImage = HatUtility.CreateHatSprite(FlipResource, fromDisk);
        }
        if (BackFlipResource != null)
        {
            cosmetic.BackFlipImage = HatUtility.CreateHatSprite(BackFlipResource, fromDisk);
        }
        if (Animation != null)
        {
            var directoryPath = Path.Combine(HatUtility.HatsAnimationsDirectory, Name);
            cosmetic.Animation = Animation.Select(
                x => HatUtility.CreateHatSprite(
                    Path.Combine(directoryPath, x), fromDisk)
            ).ToList();
            cosmetic.Time = cosmetic.Frame = 0;
        }

        if (testOnly)
        {
            // TODO : handle test hat
        }
        else
        {
            HatUtility.CustomHatRegistry.Add(hat.name, cosmetic);
        }
        HatUtility.CustomHatViewDatas.TryAdd(hat.name, viewData);
        hat.ViewDataRef = new AssetReference(viewData.Pointer);
        hat.CreateAddressableAsset();

        return hat;
    }
}