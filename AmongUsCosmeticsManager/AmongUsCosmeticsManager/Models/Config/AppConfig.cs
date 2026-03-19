namespace AmongUsCosmeticsManager.Models.Config;

public class AppConfig
{
    public string AppName { get; set; } = "Cosmetics Bundle Manager";
    public string Version { get; set; } = "1.0.0";
    public string Subtitle { get; set; } = string.Empty;
    public string CompileButtonLabel { get; set; } = "Compile";
    public string NewBundleLabel { get; set; } = "New Bundle";
    public string AddItemLabel { get; set; } = "+ Add";
    public string DeleteItemLabel { get; set; } = "Delete";
    public string NoBundleSelectedText { get; set; } = "Select a bundle";
    public string NoItemSelectedText { get; set; } = "Select an item";
    public string NoFileText { get; set; } = "No file";
    public string NoPreviewText { get; set; } = "No preview";
}
