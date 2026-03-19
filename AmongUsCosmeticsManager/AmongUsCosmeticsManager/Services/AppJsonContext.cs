using System.Text.Json.Serialization;
using AmongUsCosmeticsManager.Models.Config;

namespace AmongUsCosmeticsManager.Services;

[JsonSerializable(typeof(AppConfig))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    AllowTrailingCommas = true)]
internal partial class AppJsonContext : JsonSerializerContext;
