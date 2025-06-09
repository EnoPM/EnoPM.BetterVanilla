using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace BetterVanilla.Cosmetics.Api.Serialization;

[UsedImplicitly]
public sealed class SerializedCosmeticsManifest
{
    [JsonPropertyName("bundles")]
    public List<SerializedResourceFile>? Bundles { get; set; }
    
    [JsonPropertyName("spritesheet_files")]
    public List<SerializedResourceFile>? SpritesheetFiles { get; set; }
    
    [JsonPropertyName("hats")]
    public List<SerializedHat>? Hats { get; set; } = null!;
}