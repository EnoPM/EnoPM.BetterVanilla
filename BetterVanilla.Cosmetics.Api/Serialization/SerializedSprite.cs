using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace BetterVanilla.Cosmetics.Api.Serialization;

[UsedImplicitly]
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