using System.Collections.Generic;

namespace BetterVanilla.Cosmetics.Api.Visors;

public interface IVisorResources<TResource>
{
    public TResource MainResource { get; set; }
    public TResource PreviewResource { get; set; }
    
    public TResource? LeftResource { get; set; }
    
    public TResource? ClimbResource { get; set; }
    
    public TResource? FloorResource { get; set; }
    
    public List<TResource>? FrontAnimationFrames { get; set; }
}