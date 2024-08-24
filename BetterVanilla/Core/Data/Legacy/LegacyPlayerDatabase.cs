using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BetterVanilla.Core.Data.Legacy;

public sealed class LegacyPlayerDatabase
{
    [JsonPropertyName("a")]
    public string PlayerName { get; set; }

    [JsonPropertyName("b")]
    public uint PlayerExp { get; set; }

    [JsonPropertyName("c")]
    public uint PlayerLevel { get; set; }

    [JsonPropertyName("d")]
    public HashSet<string> FeatureCodes { get; set; } = [];
}