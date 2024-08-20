using System.Collections.Generic;
using System.Text.Json.Serialization;
using AmongUs.Data;

namespace EnoPM.BetterVanilla.Core.Data.Database;

public sealed class OutfitsDatabase
{
    [JsonPropertyName("a")]
    public List<DressingOutfit> Outfits { get; set; } = [];
}

public sealed class DressingOutfit
{
    [JsonPropertyName("b")]
    public string Hat { get; set; }

    [JsonPropertyName("c")]
    public string Skin { get; set; }

    [JsonPropertyName("d")]
    public string Visor { get; set; }

    [JsonPropertyName("e")]
    public string Pet { get; set; }

    [JsonPropertyName("f")]
    public string Nameplate { get; set; }
    
    public void ApplyOutfitToLocalPlayer()
    {
        DB.ApplyOutfit(this);
    }

    public void Delete()
    {
        DB.DeleteOutfit(this);
    }

    public bool IsEquipped()
    {
        if (DataManager.Player.Customization.Hat != Hat) return false;
        if (DataManager.Player.Customization.Skin != Skin) return false;
        if (DataManager.Player.Customization.Visor != Visor) return false;
        if (DataManager.Player.Customization.Pet != Pet) return false;
        if (DataManager.Player.Customization.NamePlate != Nameplate) return false;
        return true;
    }

    public bool IsSame(DressingOutfit other)
    {
        if (Hat != other.Hat) return false;
        if (Skin != other.Skin) return false;
        if (Visor != other.Visor) return false;
        if (Pet != other.Pet) return false;
        if (Nameplate != other.Nameplate) return false;
        return true;
    }
}