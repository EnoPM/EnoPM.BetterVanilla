using System.Collections.Generic;
using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Api.Core;
using BetterVanilla.Cosmetics.Api.Core.Serialization;

namespace BetterVanilla.Cosmetics.Api.Hats;

public sealed class SerializedHat : SerializedCosmetic, IHat<SerializedSprite>
{
    [JsonPropertyName("a")]
    public bool Bounce { get; set; }
    
    [JsonPropertyName("b")]
    public bool NoVisors { get; set; }

    [JsonPropertyName("c")]
    public SerializedSprite MainResource { get; set; } = null!;
    
    [JsonPropertyName("d")]
    public SerializedSprite PreviewResource { get; set; } = null!;
    
    [JsonPropertyName("e")]
    public SerializedSprite? FlipResource { get; set; }

    [JsonPropertyName("f")]
    public SerializedSprite? BackResource { get; set; }
    
    [JsonPropertyName("g")]
    public SerializedSprite? BackFlipResource { get; set; }
    
    [JsonPropertyName("h")]
    public SerializedSprite? ClimbResource { get; set; }
    
    [JsonPropertyName("i")]
    public List<SerializedSprite>? FrontAnimationFrames { get; set; }
    
    [JsonPropertyName("j")]
    public List<SerializedSprite>? BackAnimationFrames { get; set; }
}