using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BetterVanilla.Core.Data;

public sealed class LocalData
{
    [JsonPropertyName("player_exp")]
    public uint PlayerExp { get; set; }

    [JsonPropertyName("player_level")]
    public uint PlayerLevel { get; set; }

    [JsonPropertyName("feature_codes")]
    public HashSet<string> FeatureCodes { get; set; } = [];

    [JsonPropertyName("saved_outfits")]
    public List<LocalOutfitData> Outfits { get; set; } = [];

    [JsonPropertyName("current_preset")]
    public LocalPresetData CurrentPreset { get; set; } = new();

    [JsonPropertyName("presets")]
    public List<LocalPresetData> Presets { get; set; } = [];
}