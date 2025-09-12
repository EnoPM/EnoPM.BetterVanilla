using System.Collections.Generic;
using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Api.Core;
using BetterVanilla.Cosmetics.Api.Core.Serialization;

namespace BetterVanilla.Cosmetics.Api.Visors;

public sealed class SerializedVisor : SerializedCosmetic, IVisor<SerializedSprite>
{
    [JsonPropertyName("a")]
    public SerializedSprite MainResource { get; set; } = null!;
    
    [JsonPropertyName("b")]
    public SerializedSprite PreviewResource { get; set; } = null!;
    
    [JsonPropertyName("c")]
    public SerializedSprite? LeftResource { get; set; }
    
    [JsonPropertyName("d")]
    public SerializedSprite? ClimbResource { get; set; }
    
    [JsonPropertyName("e")]
    public SerializedSprite? FloorResource { get; set; }
    
    [JsonPropertyName("f")]
    public List<SerializedSprite>? FrontAnimationFrames { get; set; }

    [JsonPropertyName("g")]
    public bool BehindHats { get; set; }
}