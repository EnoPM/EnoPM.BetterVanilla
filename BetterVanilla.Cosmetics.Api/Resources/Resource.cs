using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Api.Resources;

public abstract class Resource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = null!;
}