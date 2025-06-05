using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Api.Cosmetics;

public class CosmeticAuthor
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}