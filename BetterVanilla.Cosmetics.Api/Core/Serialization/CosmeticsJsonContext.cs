using System.Collections.Generic;
using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Api.Hats;
using BetterVanilla.Cosmetics.Api.NamePlates;
using BetterVanilla.Cosmetics.Api.Visors;

namespace BetterVanilla.Cosmetics.Api.Core.Serialization;

[JsonSerializable(typeof(SerializedSprite))]
[JsonSerializable(typeof(SerializedCosmetic))]
[JsonSerializable(typeof(SerializedCosmeticAuthor))]
[JsonSerializable(typeof(SerializedHat))]
[JsonSerializable(typeof(SerializedVisor))]
[JsonSerializable(typeof(SerializedNamePlate))]
[JsonSerializable(typeof(List<SerializedHat>))]
[JsonSerializable(typeof(List<SerializedVisor>))]
[JsonSerializable(typeof(List<SerializedNamePlate>))]
public partial class CosmeticsJsonContext : JsonSerializerContext;