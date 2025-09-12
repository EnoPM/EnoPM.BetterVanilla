using System.Collections.Generic;

namespace BetterVanilla.Cosmetics.Api.Hats;

public interface IHatResources<TResource>
{
    public TResource MainResource { get; set; }
    public TResource PreviewResource { get; set; }
    public TResource? FlipResource { get; set; }
    public TResource? BackResource { get; set; }
    public TResource? BackFlipResource { get; set; }
    public TResource? ClimbResource { get; set; }
    
    public List<TResource>? FrontAnimationFrames { get; set; }
    public List<TResource>? BackAnimationFrames { get; set; }
}