using System.Collections.Generic;

namespace AmongUsCosmeticsManager.Models.Dto;

public class FrameListDto
{
    public string Id { get; set; } = string.Empty;
    public List<byte[]> Frames { get; set; } = [];
}
