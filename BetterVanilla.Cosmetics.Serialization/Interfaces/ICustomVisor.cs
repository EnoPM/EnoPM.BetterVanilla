namespace BetterVanilla.Cosmetics.Serialization.Interfaces;

public interface ICustomVisor<TResource, TAnimationResource> : ICosmeticItem<TResource, TAnimationResource>
    where TAnimationResource : IFrameAnimation<TResource>
{
    public bool IsBehindHat { get; set; }

    public TResource? Front { get; set; }
    public TAnimationResource? FrontAnimation { get; set; }
    
    public TResource? Left { get; set; }
    public TAnimationResource? LeftAnimation { get; set; }
    
    public TResource? Floor { get; set; }
    public TAnimationResource? FloorAnimation { get; set; }
}