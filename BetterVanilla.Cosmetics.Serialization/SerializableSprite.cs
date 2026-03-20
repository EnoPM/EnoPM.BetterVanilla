namespace BetterVanilla.Cosmetics.Serialization;

public sealed class SerializableSprite
{
    public string Path { get; set; } = string.Empty;
    public byte[]? Data { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    /// <summary>
    /// Per-frame duration in milliseconds. If null, uses the animation's default FPS.
    /// </summary>
    public int? DurationMs { get; set; }
}