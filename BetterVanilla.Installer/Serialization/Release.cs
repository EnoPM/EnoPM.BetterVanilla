using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace BetterVanilla.Installer.Serialization;

[UsedImplicitly]
public sealed class Release
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("tag_name")]
    public string Tag { get; set; } = null!;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("draft")]
    public bool Draft { get; set; }
    
    [JsonPropertyName("prerelease")]
    public bool Prerelease { get; set; }
    
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = null!;
    
    [JsonPropertyName("published_at")]
    public string PublishedAt { get; set; } = null!;
    
    [JsonPropertyName("body")]
    public string Description { get; set; } = null!;
    
    [JsonPropertyName("assets")]
    public List<Asset> Assets { get; set; } = null!;
}