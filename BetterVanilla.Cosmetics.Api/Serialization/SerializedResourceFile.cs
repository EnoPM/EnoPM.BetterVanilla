using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Api.Core;

namespace BetterVanilla.Cosmetics.Api.Serialization;

public sealed class SerializedResourceFile : IResourceFile
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = null!;
    
    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; } = null!;
}