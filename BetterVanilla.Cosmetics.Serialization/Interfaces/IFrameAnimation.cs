namespace BetterVanilla.Cosmetics.Serialization.Interfaces;

public interface IFrameAnimation<TResource>
{
    public int Fps { get; set; }

    public TResource[] Frames { get; set; }
}