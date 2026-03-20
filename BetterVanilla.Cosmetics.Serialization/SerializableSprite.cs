namespace BetterVanilla.Cosmetics.Serialization;

public sealed class SerializableSprite
{
    public string Path { get; set; } = string.Empty;
    public byte[]? Data { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}