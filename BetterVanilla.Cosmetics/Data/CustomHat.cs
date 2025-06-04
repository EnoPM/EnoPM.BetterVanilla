using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Data;

public class CustomHat
{
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("bounce")]
    public bool Bounce { get; set; }

    [JsonPropertyName("climbresource")]
    public string? ClimbResource { get; set; }
    
    [JsonPropertyName("reshashc")]
    public string? ClimbResourceHash { get; set; }

    [JsonPropertyName("condition")]
    public string? Condition { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("package")]
    public string? Package { get; set; }

    [JsonPropertyName("resource")]
    public string? Resource { get; set; }
    
    [JsonPropertyName("reshasha")]
    public string? ResourceHash { get; set; }

    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }

    [JsonPropertyName("behind")]
    public bool Behind { get; set; }

    [JsonPropertyName("backresource")]
    public string? BackResource { get; set; }
    
    [JsonPropertyName("reshashb")]
    public string? BackResourceHash { get; set; }

    [JsonPropertyName("backflipresource")]
    public string? BackFlipResource { get; set; }
    
    [JsonPropertyName("reshashbf")]
    public string? BackFlipResourceHash { get; set; }

    [JsonPropertyName("flipresource")]
    public string? FlipResource { get; set; }
    
    [JsonPropertyName("reshashf")]
    public string? FlipResourceHash { get; set; }
    
    internal string? HatsDirectoryPath { get; set; }
}