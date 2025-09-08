using System.Text.Json.Serialization;

namespace BetterVanilla.Cosmetics.Api.Core.Serialization;

public sealed class SerializedSprite
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = null!;
    
    [JsonPropertyName("x")]
    public int X { get; set; }
    
    [JsonPropertyName("y")]
    public int Y { get; set; }
    
    [JsonPropertyName("w")]
    public int Width { get; set; }
    
    [JsonPropertyName("h")]
    public int Height { get; set; }
}