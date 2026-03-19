using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AmongUsCosmeticsManager.Services.BundleImport;

[JsonSerializable(typeof(List<ImportHat>))]
[JsonSerializable(typeof(List<ImportVisor>))]
[JsonSerializable(typeof(List<ImportNamePlate>))]
internal partial class ImportJsonContext : JsonSerializerContext;
