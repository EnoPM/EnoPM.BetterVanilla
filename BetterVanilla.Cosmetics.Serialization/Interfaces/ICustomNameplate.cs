namespace BetterVanilla.Cosmetics.Serialization.Interfaces;

public interface ICustomNameplate<TResource, TAnimation> : ICosmeticItem<TResource, TAnimation>
{
    public TResource? Resource { get; set; }
    public TAnimation? ResourceAnimation { get; set; }
}