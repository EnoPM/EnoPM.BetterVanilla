using System.Text.Json.Serialization;
using BetterVanilla.Cosmetics.Api.Cosmetics;

namespace BetterVanilla.Cosmetics.Api;

public class Hat : BaseCosmetic
{
    [JsonPropertyName("bounce")]
    public bool Bounce { get; set; }

    [JsonPropertyName("main_resource")]
    public Resource MainResource { get; set; } = null!;
    
    [JsonPropertyName("back_resource")]
    public Resource? BackResource { get; set; }
    
    [JsonPropertyName("back_flip_resource")]
    public Resource? BackFlipResource { get; set; }
    
    [JsonPropertyName("climb_resource")]
    public Resource? ClimbResource { get; set; }
}