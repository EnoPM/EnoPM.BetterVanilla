namespace BetterVanilla.Cosmetics.Serialization.Interfaces;

public interface IFrameAnimation<TStep>
{
    public int Fps { get; set; }

    public TStep[] Steps { get; set; }
}