using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Api;

public class CosmeticRegistry
{
    [JsonPropertyName("hats")]
    public List<HatPackage>? Hats { get; set; }
}