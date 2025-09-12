using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Core.Data;

public sealed class CosmeticsBundleVersion
{
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = null!;

    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; } = null!;
}