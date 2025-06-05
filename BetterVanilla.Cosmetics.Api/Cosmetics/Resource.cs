using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Api.Cosmetics;

public class Resource
{
    [JsonPropertyName("type"), JsonConverter(typeof(JsonStringEnumConverter))]
    public ResourceType Type { get; set; }
    
    [JsonPropertyName("path")]
    public string Path { get; set; } = null!;
    
    [JsonPropertyName("file_hash")]
    public string? FileHash { get; set; }
}