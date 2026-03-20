using BetterVanilla.Cosmetics.Serialization.Interfaces;

namespace BetterVanilla.Cosmetics.Serialization;

public class SerializableFrameAnimation : IFrameAnimation<SerializableSprite>
{
    public int Fps { get; set; }
    public SerializableSprite[] Frames { get; set; } = [];
}