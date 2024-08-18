using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnoPM.BetterVanilla.Data.Database;

public sealed class PresetsDatabase
{
    [JsonPropertyName("a")]
    public SerializedPreset Current { get; set; } = new();
    
    [JsonPropertyName("b")]
    public Dictionary<string, SerializedPreset> Presets { get; set; } = [];
}

public sealed class SerializedPreset
{
    [JsonPropertyName("a")]
    public SerializedPresetData Local { get; set; } = new();
    
    [JsonPropertyName("b")]
    public SerializedPresetData Shared { get; set; } = new();
}

public sealed class SerializedPresetData
{
    [JsonPropertyName("a")]
    public Dictionary<string, bool> BoolStore { get; set; } = [];

    [JsonPropertyName("b")]
    public Dictionary<string, string> StringStore { get; set; } = [];

    [JsonPropertyName("c")]
    public Dictionary<string, float> FloatStore { get; set; } = [];
}