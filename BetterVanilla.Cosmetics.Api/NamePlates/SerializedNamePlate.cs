using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Api.Core;
using BetterVanilla.Cosmetics.Api.Core.Serialization;

namespace BetterVanilla.Cosmetics.Api.NamePlates;

public class SerializedNamePlate : SerializedCosmetic, INamePlate<SerializedSprite>
{
    [JsonPropertyName("main_resource")]
    public SerializedSprite MainResource { get; set; } = null!;
}