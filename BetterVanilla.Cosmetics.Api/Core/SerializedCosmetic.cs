using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Api.Core.Serialization;

namespace BetterVanilla.Cosmetics.Api.Core;

public abstract class SerializedCosmetic : ICosmeticItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }
    
    [JsonPropertyName("author")]
    public SerializedCosmeticAuthor? Author { get; set; }
}