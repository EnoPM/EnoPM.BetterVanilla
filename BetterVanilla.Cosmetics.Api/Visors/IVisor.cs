using BetterVanilla.Cosmetics.Api.Core;

namespace BetterVanilla.Cosmetics.Api.Visors;

public interface IVisor<TResource> : IVisorResources<TResource>, ICosmeticItem
{
    public bool BehindHats { get; set; }
}