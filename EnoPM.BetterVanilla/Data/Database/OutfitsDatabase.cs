using System.Collections.Generic;
using System.Text.Json.Serialization;
using EnoPM.BetterVanilla.Core;

namespace EnoPM.BetterVanilla.Data.Database;

public sealed class OutfitsDatabase
{
    [JsonPropertyName("a")]
    public List<DressingOutfit> Outfits { get; set; } = [];
}

public sealed class DressingOutfit
{
    [JsonPropertyName("a")]
    public string Name { get; set; }

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
        DB.ApplyOutfit(Name);
    }

    public void Delete()
    {
        DB.DeleteOutfit(Name);
    }
}