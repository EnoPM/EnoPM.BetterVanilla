using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace BetterVanilla.Cosmetics.Api.Resources;

[UsedImplicitly]
public class AssetBundleEntry
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("path")]
    public string Path { get; set; } = null!;
}