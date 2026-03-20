namespace BetterVanilla.Cosmetics.Serialization.Interfaces;

public interface ICustomHat<TResource, TAnimation> : ICosmeticItem<TResource, TAnimation>
{
    public bool IsBounce { get; set; }
    public bool DisableVisors { get; set; }
    
    public TResource? Front { get; set; }
    public TAnimation? FrontAnimation { get; set; }

    public TResource? Flip { get; set; }
    public TAnimation? FlipAnimation { get; set; }

    public TResource? Back { get; set; }
    public TAnimation? BackAnimation { get; set; }

    public TResource? BackFlip { get; set; }
    public TAnimation? BackFlipAnimation { get; set; }

    public TResource? Climb { get; set; }
    public TAnimation? ClimbAnimation { get; set; }
}