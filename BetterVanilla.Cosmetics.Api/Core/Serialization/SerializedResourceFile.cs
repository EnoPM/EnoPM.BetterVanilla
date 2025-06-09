using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Api.Core.Serialization;

public sealed class SerializedResourceFile : IResourceFile
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = null!;
    
    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; } = null!;
}