using System.Collections.Generic;
using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Api.Core;
using BetterVanilla.Cosmetics.Api.Core.Serialization;

namespace BetterVanilla.Cosmetics.Api.Visors;

public sealed class SerializedVisor : SerializedCosmetic, IVisor<SerializedSprite>
{
    [JsonPropertyName("main_resource")]
    public SerializedSprite MainResource { get; set; } = null!;
    
    [JsonPropertyName("preview_resource")]
    public SerializedSprite PreviewResource { get; set; } = null!;
    
    [JsonPropertyName("left_resource")]
    public SerializedSprite? LeftResource { get; set; }
    
    [JsonPropertyName("climb_resource")]
    public SerializedSprite? ClimbResource { get; set; }
    
    [JsonPropertyName("floor_resource")]
    public SerializedSprite? FloorResource { get; set; }
    
    [JsonPropertyName("front_animation_frames")]
    public List<SerializedSprite>? FrontAnimationFrames { get; set; }

    [JsonPropertyName("behind_hats")]
    public bool BehindHats { get; set; }
}