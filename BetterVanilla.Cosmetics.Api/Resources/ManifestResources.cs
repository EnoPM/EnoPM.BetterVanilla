using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace BetterVanilla.Cosmetics.Api.Resources;

[UsedImplicitly]
public sealed class ManifestResources
{
    [JsonPropertyName("png")]
    public List<PngFile> Png { get; set; } = [];
    
    [JsonPropertyName("asset_bundle")]
    public List<AssetBundleFile> AssetBundle { get; set; } = [];
}