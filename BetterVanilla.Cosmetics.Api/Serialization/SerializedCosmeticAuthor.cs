using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Api.Serialization;

public sealed class SerializedCosmeticAuthor
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}