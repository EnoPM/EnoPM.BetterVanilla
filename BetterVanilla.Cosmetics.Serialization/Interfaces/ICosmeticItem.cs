namespace BetterVanilla.Cosmetics.Serialization.Interfaces;

public interface ICosmeticItem<TResource, TAnimationResource> where TAnimationResource : IFrameAnimation<TResource>
{
    public string Name { get; set; }
    
    public bool IsAdaptive { get; set; }
    
    public SerializableAuthor? Author { get; set; }
    
    public TResource? Preview { get; set; }
    public TAnimationResource? PreviewAnimation { get; set; }
}