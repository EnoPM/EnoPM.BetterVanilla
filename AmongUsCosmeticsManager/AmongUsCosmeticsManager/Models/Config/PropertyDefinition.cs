namespace AmongUsCosmeticsManager.Models.Config;

public class PropertyDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "bool";
    public object? Default { get; set; }
}
