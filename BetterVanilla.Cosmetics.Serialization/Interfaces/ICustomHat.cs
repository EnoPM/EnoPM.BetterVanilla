namespace BetterVanilla.Cosmetics.Serialization.Interfaces;

public interface ICustomHat<TResource, TAnimationResource> : ICosmeticItem<TResource, TAnimationResource> where TAnimationResource : IFrameAnimation<TResource>
{
    public bool IsBounce { get; set; }
    public bool DisableVisors { get; set; }
    
    public TResource? Front { get; set; }
    public TAnimationResource? FrontAnimation { get; set; }
    
    public TResource? Flip { get; set; }
    public TAnimationResource? FlipAnimation { get; set; }
    
    public TResource? Back { get; set; }
    public TAnimationResource? BackAnimation { get; set; }
    
    public TResource? BackFlip { get; set; }
    public TAnimationResource? BackFlipAnimation { get; set; }
    
    public TResource? Climb { get; set; }
    public TAnimationResource? ClimbAnimation { get; set; }
}