using BetterVanilla.Cosmetics.Serialization.Core;
using BetterVanilla.Cosmetics.Serialization.Interfaces;

namespace BetterVanilla.Cosmetics.Serialization;

public sealed class SerializableHat : SerializableCosmeticBase, ICustomHat<SerializableSprite, SerializableFrameAnimation>
{
    public bool IsBounce { get; set; }
    public bool DisableVisors { get; set; }

    public SerializableSprite? Front { get; set; }
    public SerializableFrameAnimation? FrontAnimation { get; set; }

    public SerializableSprite? Flip { get; set; }
    public SerializableFrameAnimation? FlipAnimation { get; set; }

    public SerializableSprite? Back { get; set; }
    public SerializableFrameAnimation? BackAnimation { get; set; }

    public SerializableSprite? BackFlip { get; set; }
    public SerializableFrameAnimation? BackFlipAnimation { get; set; }

    public SerializableSprite? Climb { get; set; }
    public SerializableFrameAnimation? ClimbAnimation { get; set; }
}