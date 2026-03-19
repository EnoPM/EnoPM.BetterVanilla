using SkiaSharp;

namespace AmongUsCosmeticsManager.Models;

public record PlayerColor(string Name, SKColor Body, SKColor Shadow);

public static class PlayerPalette
{
    private static SKColor Hex(int v) => new((byte)(v >> 16), (byte)(v >> 8), (byte)v, 255);

    public static readonly PlayerColor[] Colors =
    [
        new("Red",       new SKColor(198, 17, 17),   new SKColor(122, 8, 56)),
        new("Blue",      new SKColor(19, 46, 210),    new SKColor(9, 21, 142)),
        new("Green",     new SKColor(17, 128, 45),    new SKColor(10, 77, 46)),
        new("Pink",      new SKColor(238, 84, 187),   new SKColor(172, 43, 174)),
        new("Orange",    new SKColor(240, 125, 13),   new SKColor(180, 62, 21)),
        new("Yellow",    new SKColor(246, 246, 87),   new SKColor(195, 136, 34)),
        new("Black",     new SKColor(63, 71, 78),     new SKColor(30, 31, 38)),
        new("White",     new SKColor(215, 225, 241),  new SKColor(132, 149, 192)),
        new("Purple",    new SKColor(107, 47, 188),   new SKColor(59, 23, 124)),
        new("Brown",     new SKColor(113, 73, 30),    new SKColor(94, 38, 21)),
        new("Cyan",      new SKColor(56, 255, 221),   new SKColor(36, 169, 191)),
        new("Lime",      new SKColor(80, 240, 57),    new SKColor(21, 168, 66)),
        new("Maroon",    Hex(6233390),                Hex(4263706)),
        new("Rose",      Hex(15515859),               Hex(14586547)),
        new("Banana",    Hex(15787944),               Hex(13810825)),
        new("Gray",      Hex(7701907),                Hex(4609636)),
        new("Tan",       Hex(9537655),                Hex(5325118)),
        new("Coral",     Hex(14115940),               Hex(11813730))
    ];
}
