using System.Collections.Generic;
using System.IO;
using System.Linq;
using BetterVanilla.BetterModMenu.Core;
using BetterVanilla.Core;
using BetterVanilla.Core.Data;
using UnityEngine.UI;

namespace BetterVanilla.BetterModMenu.Tabs;

public sealed class OutfitSaverTabUi : BaseMenuTabUi
{
    public Button saveCurrentOutfitButton = null!;
    public SavedOutfitItemUi outfitItemPrefab = null!;

    private List<SavedOutfitItemUi> AllOutfitUis { get; } = [];

    public void OnSaveCurrentOutfitButtonClicked()
    {
        if (CurrentOutfitIsEquipped()) return;
        var player = PlayerControl.LocalPlayer;
        if (player == null
            || player.Data == null
            || player.Data.DefaultOutfit == null) return;
        var outfit = new SerializableOutfit(player.Data.DefaultOutfit);
        var outfitUi = Instantiate(outfitItemPrefab, tabContent);
        outfitUi.Outfit = outfit;
        AllOutfitUis.Add(outfitUi);
        SaveOutfits();
    }

    public void DeleteOutfit(SerializableOutfit outfit)
    {
        var item = AllOutfitUis.FirstOrDefault(x => x.Outfit == outfit);
        if (item == null) return;
        Destroy(item.gameObject);
        AllOutfitUis.Remove(item);
        SaveOutfits();
    }
    
    private void Start()
    {
        if (!File.Exists(ModPaths.SavedOutfitsFile)) return;
        using var file = File.OpenRead(ModPaths.SavedOutfitsFile);
        using var reader = new BinaryReader(file);
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var outfit = new SerializableOutfit(reader);
            var outfitUi = Instantiate(outfitItemPrefab, tabContent);
            outfitUi.Outfit = outfit;
            AllOutfitUis.Add(outfitUi);
        }
    }

    private void Update()
    {
        saveCurrentOutfitButton.interactable = !CurrentOutfitIsEquipped();
        var canApply = !LocalConditions.IsGameStarted();
        foreach (var outfit in AllOutfitUis)
        {
            outfit.SetApplyButtonEnabled(canApply);
        }
    }

    private bool CurrentOutfitIsEquipped()
    {
        foreach (var outfit in AllOutfitUis)
        {
            if (outfit.Outfit == null) continue;
            if (outfit.Outfit.IsEquipped())
            {
                return true;
            }
        }
        return false;
    }

    private void SaveOutfits()
    {
        using var file = File.Create(ModPaths.SavedOutfitsFile);
        using var writer = new BinaryWriter(file);
        var allOutfits = AllOutfitUis.Where(x => x.Outfit != null)
            .Select(x => x.Outfit)
            .ToList();
        writer.Write(allOutfits.Count);
        foreach (var outfit in allOutfits)
        {
            outfit!.Write(writer);
        }
    }
}