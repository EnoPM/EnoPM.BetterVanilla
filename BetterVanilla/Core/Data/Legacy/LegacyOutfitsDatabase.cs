using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BetterVanilla.Core.Data.Legacy;

public sealed class LegacyOutfitsDatabase
{
    [JsonPropertyName("a")]
    public List<LegacyDressingOutfit> Outfits { get; set; } = [];
}