namespace BetterVanilla.Cosmetics.Serialization;

public sealed class SerializableAnimationStep
{
    /// <summary>
    /// 0 = Frame (has sprite data), 1 = Delay (no sprite, just wait)
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// Duration in milliseconds for this step.
    /// For frames: null means use the animation's default FPS-based duration.
    /// For delays: always set.
    /// </summary>
    public int? DurationMs { get; set; }

    /// <summary>
    /// Sprite data for frame steps. Null for delay steps.
    /// </summary>
    public SerializableSprite? Sprite { get; set; }
}
