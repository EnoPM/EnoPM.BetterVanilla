using System.Collections.Generic;

namespace AmongUsCosmeticsManager.Models.Dto;

public class ItemDto
{
    public string TypeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public long LastModifiedTicks { get; set; }
    public List<PropertyDto> Properties { get; set; } = [];
    public List<ResourceDto> Resources { get; set; } = [];
    public List<FrameListDto> FrameLists { get; set; } = [];
}
