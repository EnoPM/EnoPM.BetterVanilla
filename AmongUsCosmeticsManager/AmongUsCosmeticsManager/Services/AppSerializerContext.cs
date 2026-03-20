using BSerializer;
using BetterVanilla.Cosmetics.Serialization;
using AmongUsCosmeticsManager.Models.Dto;

namespace AmongUsCosmeticsManager.Services;

[BSerializable(typeof(ProjectDto))]
[BSerializable(typeof(SerializableBundle))]
public partial class AppSerializerContext : BSerializerContext;
