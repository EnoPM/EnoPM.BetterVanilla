using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Data;

public class ManifestFile
{
    [JsonPropertyName("hats")]
    public List<CustomHat>? Hats { get; set; }
}