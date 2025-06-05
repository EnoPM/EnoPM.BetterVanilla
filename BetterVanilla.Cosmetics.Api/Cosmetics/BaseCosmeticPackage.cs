using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Api.Cosmetics;

public abstract class BaseCosmeticPackage
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}