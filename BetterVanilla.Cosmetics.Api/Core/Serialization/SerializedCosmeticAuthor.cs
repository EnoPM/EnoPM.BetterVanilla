using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Api.Core.Serialization;

public sealed class SerializedCosmeticAuthor
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}