using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BetterVanilla.Cosmetics.Api.Serialization;
using BetterVanilla.Cosmetics.Core.Manager;
using BetterVanilla.Cosmetics.Core.Spritesheet;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BetterVanilla.Cosmetics.Hats;

public sealed class HatCosmeticManager : BaseCosmeticManager<HatCosmetic, HatViewData, HatParent, HatData>
{
    public const string CustomPackageName = "Better Vanilla";
    public const string InnerslothPackageName = "Innersloth";
    
    protected override bool CanBeCached(HatParent parent)
    {
        return parent != null && parent.Hat != null;
    }

    protected override void HydrateViewData(HatViewData viewData, HatCosmetic cosmetic)
    {
        CosmeticsPlugin.Logging.LogMessage($"Hydrating {cosmetic.Name}: {nameof(cosmetic.MainResource)} {cosmetic.MainResource != null}");
        viewData.name = $"{cosmetic.Name}HatViewData";
        viewData.FloorImage = viewData.MainImage = cosmetic.MainResource;

        if (cosmetic.BackResource != null)
        {
            viewData.BackImage = cosmetic.BackResource;
            //viewData.LeftBackImage = cosmetic.BackResource;
            cosmetic.Behind = true;
        }

        if (cosmetic.ClimbResource != null)
        {
            viewData.ClimbImage = viewData.LeftClimbImage = cosmetic.ClimbResource;
        }

        if (cosmetic.Adaptive)
        {
            viewData.MatchPlayerColor = CachedMaterial != null;
        }
    }

    protected override void HydrateCosmeticData(HatData cosmeticData, HatCosmetic cosmetic)
    {
        cosmeticData.name = cosmetic.Name;
        cosmeticData.displayOrder = 99;
        cosmeticData.ProductId = GetCosmeticProductId(cosmetic);
        cosmeticData.InFront = !cosmetic.Behind;
        cosmeticData.NoBounce = !cosmetic.Bounce;
        cosmeticData.ChipOffset = new Vector2(0f, 0f); // TODO: edit offset
        cosmeticData.Free = true;

        // TODO: Check if it's really useful
        cosmeticData.ViewDataRef = new AssetReference(ViewDataCache[cosmeticData.name].Pointer);
        //cosmeticData.CreateAddressableAsset();
    }
    protected override string GetCosmeticProductId(HatCosmetic cosmetic)
    {
        return $"hat_bv_{cosmetic.Name.Replace(' ', '_')}";
    }
    
    protected override bool IsParentCosmetic(HatParent parent, HatCosmetic cosmetic)
    {
        return parent != null
               && parent.Hat != null
               && parent.Hat.ProductId == GetCosmeticProductId(cosmetic);
    }
    
    protected override void PopulateParent(HatParent parent)
    {
        parent.PopulateFromViewData();
    }

    protected override HatParent? GetPlayerParent(PlayerControl player)
    {
        if (player == null || player.Data == null || player.cosmetics == null)
        {
            return null;
        }
        return player.cosmetics.hat;
    }

    public BaseSpritesheet OpenFileSpritesheet(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Spritesheet path '{filePath}' not found");
        }
        var spritesheet = new FileSpritesheet(filePath);

        return spritesheet;
    }

    public SerializedHat OpenCosmeticFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Cosmetic path '{filePath}' not found");
        }
        var fileContent = File.ReadAllText(filePath);
        var serializedCosmetic = JsonSerializer.Deserialize<SerializedHat>(fileContent);
        if (serializedCosmetic == null)
        {
            throw new Exception("Cosmetic could not be deserialized");
        }
        
        return serializedCosmetic;
    }

    public HatCosmetic CreateCosmetic(BaseSpritesheet spritesheet, SerializedHat serializedHat)
    {
        var cosmetic = new HatCosmetic(serializedHat, spritesheet);

        return cosmetic;
    }
    
    public override void AddCosmetic(HatCosmetic cosmetic)
    {
        UnregisteredCosmetics.Add(cosmetic);
    }
    
    public override void UpdateAnimationFrames()
    {
        foreach (var cosmetic in GetAllRegisteredCosmetics())
        {
            cosmetic.FrontTime += Time.deltaTime * 150;
            if (cosmetic.FrontTime >= cosmetic.FrontDelay)
            {
                cosmetic.UpdateFrontFrames();
                cosmetic.FrontTime = 0f;
            }
            cosmetic.BackTime += Time.deltaTime * 150;
            if (cosmetic.BackTime >= cosmetic.BackDelay)
            {
                cosmetic.UpdateBackFrames();
                cosmetic.BackTime = 0f;
            }
        }
    }

    protected override List<HatData> GetVanillaCosmeticData()
    {
        return HatManager.Instance.allHats.ToList();
    }
    protected override void OverrideVanillaCosmeticData(List<HatData> allCosmeticData)
    {
        HatManager.Instance.allHats = allCosmeticData.ToArray();
    }
}