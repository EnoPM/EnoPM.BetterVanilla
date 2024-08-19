using System.Text.Json.Serialization;

namespace EnoPM.BetterVanilla.Core.Data.Database;

public sealed class PlayerDatabase
{
    [JsonPropertyName("a")]
    public string PlayerName { get; set; }

    [JsonPropertyName("b")]
    public uint PlayerExp { get; set; }

    [JsonPropertyName("c")]
    public uint PlayerLevel { get; set; }
}