using System.Text.Json.Serialization;
using AmongUs.Data;

namespace BetterVanilla.Core.Data;

public sealed class LocalOutfitData
{
    [JsonPropertyName("hat")]
    public string Hat { get; set; }
    
    [JsonPropertyName("Skin")]
    public string Skin { get; set; }
    
    [JsonPropertyName("visor")]
    public string Visor { get; set; }
    
    [JsonPropertyName("pet")]
    public string Pet { get; set; }
    
    [JsonPropertyName("nameplate")]
    public string Nameplate { get; set; }

    public bool IsEquipped()
    {
        if (DataManager.Player.Customization.Hat != Hat) return false;
        if (DataManager.Player.Customization.Skin != Skin) return false;
        if (DataManager.Player.Customization.Visor != Visor) return false;
        if (DataManager.Player.Customization.Pet != Pet) return false;
        if (DataManager.Player.Customization.NamePlate != Nameplate) return false;
        return true;
    }
    
    public bool IsSame(LocalOutfitData other)
    {
        if (Hat != other.Hat) return false;
        if (Skin != other.Skin) return false;
        if (Visor != other.Visor) return false;
        if (Pet != other.Pet) return false;
        if (Nameplate != other.Nameplate) return false;
        return true;
    }
}