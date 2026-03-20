using System.Collections.Generic;

namespace AmongUsCosmeticsManager.Models.Dto;

public class AnimationNodeDto
{
    public string Type { get; set; } = "frame";
    public byte[]? Data { get; set; }
    public int? DurationMs { get; set; }
    public int? LoopCount { get; set; }
    public List<AnimationNodeDto>? Children { get; set; }
}
