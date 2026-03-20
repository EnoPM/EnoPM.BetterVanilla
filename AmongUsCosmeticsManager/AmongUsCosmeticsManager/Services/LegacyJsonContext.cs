using System.Collections.Generic;
using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Serialization;

namespace AmongUsCosmeticsManager.Services;

[JsonSerializable(typeof(List<SerializableHat>))]
[JsonSerializable(typeof(List<SerializableVisor>))]
[JsonSerializable(typeof(List<SerializableNameplate>))]
internal partial class LegacyJsonContext : JsonSerializerContext;
