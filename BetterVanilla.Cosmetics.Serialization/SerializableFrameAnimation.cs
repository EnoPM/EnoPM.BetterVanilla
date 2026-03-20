using BetterVanilla.Cosmetics.Serialization.Interfaces;

namespace BetterVanilla.Cosmetics.Serialization;

public class SerializableFrameAnimation : IFrameAnimation<SerializableAnimationStep>
{
    public int Fps { get; set; }
    public SerializableAnimationStep[] Steps { get; set; } = [];
}