using BetterVanilla.Cosmetics.Api.Serialization;

namespace BetterVanilla.Cosmetics.Api.Core;

public interface ICosmeticItem
{
    public string Name { get; set; }
    
    public bool Adaptive { get; set; }
    
    public SerializedCosmeticAuthor? Author { get; set; }
}