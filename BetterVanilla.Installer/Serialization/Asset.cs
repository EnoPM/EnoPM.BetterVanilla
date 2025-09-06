using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace BetterVanilla.Installer.Serialization;

[UsedImplicitly]
public sealed class Asset
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
    
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("size")]
    public int Size { get; set; }
    
    [JsonPropertyName("browser_download_url")]
    public string DownloadUrl { get; set; } = null!;
}