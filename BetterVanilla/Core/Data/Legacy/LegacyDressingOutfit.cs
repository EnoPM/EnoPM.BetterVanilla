using System.Text.Json.Serialization;

namespace BetterVanilla.Core.Data.Legacy;

public sealed class LegacyDressingOutfit
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
}