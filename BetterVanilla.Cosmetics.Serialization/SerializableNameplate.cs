using BetterVanilla.Cosmetics.Serialization.Core;
using BetterVanilla.Cosmetics.Serialization.Interfaces;

namespace BetterVanilla.Cosmetics.Serialization;

public sealed class SerializableNameplate : SerializableCosmeticBase, ICustomNameplate<SerializableSprite, SerializableFrameAnimation>
{
    public SerializableSprite? Resource { get; set; }
    public SerializableFrameAnimation? ResourceAnimation { get; set; }
}