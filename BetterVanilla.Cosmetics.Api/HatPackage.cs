using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Api.Cosmetics;

namespace BetterVanilla.Cosmetics.Api;

public class HatPackage : BaseCosmeticPackage
{
    [JsonPropertyName("hats")]
    public List<Hat>? Hats { get; set; }
}