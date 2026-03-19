namespace AmongUsCosmeticsManager.Models.Dto;

public class ResourceDto
{
    public string Id { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public byte[] Data { get; set; } = [];
}
