using System.Text.Json.Serialization;

namespace BetterVanilla.ToolsLib.Serialization;

[JsonSerializable(typeof(Release[]))]
[JsonSerializable(typeof(Release))]
[JsonSerializable(typeof(Asset))]
[JsonSerializable(typeof(Version))]
[JsonSerializable(typeof(BepInExVersion))]
public partial class GithubJsonContext : JsonSerializerContext;