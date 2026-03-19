using System;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace AmongUsCosmeticsManager.Services;

public static class RecolorService
{
    public static byte[] Recolor(byte[] imageData, SKColor bodyColor, SKColor visorColor, SKColor shadowColor)
    {
        using var bitmap = SKBitmap.Decode(imageData);
        if (bitmap == null) return imageData;

        // Ensure 32-bit RGBA format for direct pixel access
        using var rgba = bitmap.ColorType == SKColorType.Rgba8888
            ? bitmap
            : bitmap.Copy(SKColorType.Rgba8888);

        if (rgba == null) return imageData;

        var pixels = rgba.GetPixelSpan();
        var result = new byte[pixels.Length];
        pixels.CopyTo(result);

        var span = MemoryMarshal.Cast<byte, uint>(result.AsSpan());

        var bodyR = bodyColor.Red;
        var bodyG = bodyColor.Green;
        var bodyB = bodyColor.Blue;
        var visorR = visorColor.Red;
        var visorG = visorColor.Green;
        var visorB = visorColor.Blue;
        var shadowR = shadowColor.Red;
        var shadowG = shadowColor.Green;
        var shadowB = shadowColor.Blue;

        for (var i = 0; i < span.Length; i++)
        {
            var packed = span[i];
            var r = (byte)(packed & 0xFF);
            var g = (byte)((packed >> 8) & 0xFF);
            var b = (byte)((packed >> 16) & 0xFF);
            var a = (byte)((packed >> 24) & 0xFF);

            if (a == 0) continue;

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            // Skip desaturated pixels
            if (max < 1 || (max - min) * 255 < max * 51) continue; // 51/255 ≈ 0.20

            var brightness = max; // keep as byte, divide later

            byte outR, outG, outB;
            if (r >= g && r >= b)
            {
                // Red dominant → body
                outR = (byte)(bodyR * brightness / 255);
                outG = (byte)(bodyG * brightness / 255);
                outB = (byte)(bodyB * brightness / 255);
            }
            else if (g >= r && g >= b)
            {
                // Green dominant → visor
                outR = (byte)(visorR * brightness / 255);
                outG = (byte)(visorG * brightness / 255);
                outB = (byte)(visorB * brightness / 255);
            }
            else
            {
                // Blue dominant → shadow
                outR = (byte)(shadowR * brightness / 255);
                outG = (byte)(shadowG * brightness / 255);
                outB = (byte)(shadowB * brightness / 255);
            }

            span[i] = (uint)(outR | (outG << 8) | (outB << 16) | (a << 24));
        }

        using var resBitmap = new SKBitmap(rgba.Width, rgba.Height, SKColorType.Rgba8888, rgba.AlphaType);
        Marshal.Copy(result, 0, resBitmap.GetPixels(), result.Length);
        using var image = SKImage.FromBitmap(resBitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
