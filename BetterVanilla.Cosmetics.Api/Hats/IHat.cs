using BetterVanilla.Cosmetics.Api.Core;

namespace BetterVanilla.Cosmetics.Api.Hats;

public interface IHat<TResource> : ICosmeticItem, IHatResources<TResource>
{
    public bool Bounce { get; set; }
    
    public bool NoVisors { get; set; }
}