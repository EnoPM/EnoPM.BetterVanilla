using BetterVanilla.Cosmetics.Serialization.Interfaces;

namespace BetterVanilla.Cosmetics.Serialization.Core;

public abstract class SerializableCosmeticBase : ICosmeticItem<SerializableSprite, SerializableFrameAnimation>
{
    public string Name { get; set; } = string.Empty;

    public bool IsAdaptive { get; set; }

    public SerializableAuthor? Author { get; set; }

    public SerializableSprite? Preview { get; set; }
    public SerializableFrameAnimation? PreviewAnimation { get; set; }
}