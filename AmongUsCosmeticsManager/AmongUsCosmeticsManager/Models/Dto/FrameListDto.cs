using System.Collections.Generic;

namespace AmongUsCosmeticsManager.Models.Dto;

public class FrameListDto
{
    public string Id { get; set; } = string.Empty;
    public int DefaultFps { get; set; } = 10;
    public List<AnimationNodeDto> Nodes { get; set; } = [];

    // Legacy: kept for backward-compatible loading of old project files
    public List<byte[]> Frames { get; set; } = [];
}
