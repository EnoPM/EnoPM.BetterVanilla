namespace BetterVanilla.Cosmetics.Serialization.Interfaces;

public interface ICustomVisor<TResource, TAnimation> : ICosmeticItem<TResource, TAnimation>
{
    public bool IsBehindHat { get; set; }

    public TResource? Front { get; set; }
    public TAnimation? FrontAnimation { get; set; }

    public TResource? Left { get; set; }
    public TAnimation? LeftAnimation { get; set; }

    public TResource? Floor { get; set; }
    public TAnimation? FloorAnimation { get; set; }
}