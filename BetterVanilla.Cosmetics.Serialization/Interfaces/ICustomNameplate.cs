namespace BetterVanilla.Cosmetics.Serialization.Interfaces;

public interface ICustomNameplate<TResource, TAnimationResource> : ICosmeticItem<TResource, TAnimationResource> where TAnimationResource : IFrameAnimation<TResource>
{
    public TResource? Resource { get; set; }
    public TAnimationResource? ResourceAnimation { get; set; }
}