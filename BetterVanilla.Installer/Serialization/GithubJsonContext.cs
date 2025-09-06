using System.Text.Json.Serialization;

namespace BetterVanilla.Installer.Serialization;

[JsonSerializable(typeof(Release[]))]
[JsonSerializable(typeof(Release))]
[JsonSerializable(typeof(Asset))]
[JsonSerializable(typeof(Version))]
[JsonSerializable(typeof(BepInExVersion))]
internal partial class GithubJsonContext : JsonSerializerContext;