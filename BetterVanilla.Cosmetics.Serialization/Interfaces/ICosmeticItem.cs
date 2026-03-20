namespace BetterVanilla.Cosmetics.Serialization.Interfaces;

public interface ICosmeticItem<TResource, TAnimation>
{
    public string Name { get; set; }

    public bool IsAdaptive { get; set; }

    public SerializableAuthor? Author { get; set; }

    public TResource? Preview { get; set; }
    public TAnimation? PreviewAnimation { get; set; }
}