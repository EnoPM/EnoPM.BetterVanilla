using System.Text.Json.Serialization;

namespace EnoPM.BetterVanilla.Data.Database;

public sealed class PlayerDatabase
{
    [JsonPropertyName("a")]
    public string PlayerName { get; set; }

    [JsonPropertyName("b")]
    public long PlayerExp { get; set; }

    [JsonPropertyName("c")]
    public uint PlayerLevel { get; set; }
}