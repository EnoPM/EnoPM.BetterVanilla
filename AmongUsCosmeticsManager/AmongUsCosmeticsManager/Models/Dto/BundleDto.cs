using System.Collections.Generic;

namespace AmongUsCosmeticsManager.Models.Dto;

public class BundleDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsCompiled { get; set; }
    public long CompiledDateTicks { get; set; }
    public List<ItemDto> Items { get; set; } = [];
}
