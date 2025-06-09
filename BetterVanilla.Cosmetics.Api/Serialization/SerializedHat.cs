using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Api.Core;
using BetterVanilla.Cosmetics.Api.Hats;

namespace BetterVanilla.Cosmetics.Api.Serialization;

public sealed class SerializedHat : SerializedCosmetic, IHat<SerializedSprite>
{
    [JsonPropertyName("bounce")]
    public bool Bounce { get; set; }

    [JsonPropertyName("main_resource")]
    public SerializedSprite MainResource { get; set; } = null!;
    
    [JsonPropertyName("flip_resource")]
    public SerializedSprite? FlipResource { get; set; }

    [JsonPropertyName("back_resource")]
    public SerializedSprite? BackResource { get; set; }
    
    [JsonPropertyName("back_flip_resource")]
    public SerializedSprite? BackFlipResource { get; set; }
    
    [JsonPropertyName("climb_resource")]
    public SerializedSprite? ClimbResource { get; set; }
    
    [JsonPropertyName("front_animation_frames")]
    public List<SerializedSprite>? FrontAnimationFrames { get; set; }
    
    [JsonPropertyName("back_animation_frames")]
    public List<SerializedSprite>? BackAnimationFrames { get; set; }
}