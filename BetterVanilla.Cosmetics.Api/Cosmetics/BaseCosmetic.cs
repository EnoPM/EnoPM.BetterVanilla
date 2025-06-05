using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Api.Cosmetics;

public abstract class BaseCosmetic
{
    [JsonPropertyName("author")]
    public CosmeticAuthor? Author { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }
}