using System.Text.Json.Serialization;
using BetterVanilla.ToolsLib.Converters;
using JetBrains.Annotations;

namespace BetterVanilla.ToolsLib.Serialization;

[UsedImplicitly]
public sealed class BepInExVersion
{
    [JsonPropertyName("version"), JsonConverter(typeof(JsonVersionConverter))]
    public Version Version { get; set; } = null!;

    [JsonPropertyName("build_number")]
    public uint BuildNumber { get; set; }
    
    [JsonPropertyName("build_hash")]
    public string BuildHash { get; set; } = null!;
}