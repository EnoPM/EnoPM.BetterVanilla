using System;
using System.Globalization;
using UnityEngine;

namespace BetterVanilla.Core.Helpers;

public static class ColorUtils
{
    public static readonly Color CheaterColor = Palette.ImpostorRed;
    public static readonly Color HostColor = Color.magenta;
    public static readonly Color ImpostorColor = Palette.ImpostorRed;
    private static readonly Color NoTasksDoneColor = new(0.76f, 0.16f, 0.52f, 1f);
    private static readonly Color AllTasksDoneColor = new(0.34f, 1f, 0.69f, 1f);

    public static Color TaskCountColor(int done, int total)
    {
        var percent = (float)done / total;
        return Color.Lerp(NoTasksDoneColor, AllTasksDoneColor, percent);
    }
    
    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    public static string ColoredString(Color c, string s)
    {
        return $"<color=#{ToByte(c.r):X2}{ToByte(c.g):X2}{ToByte(c.b):X2}{ToByte(c.a):X2}>{s}</color>";
    }
    
    public static string ToHex(Color color)
    {
        var r = Mathf.RoundToInt(color.r * 255);
        var g = Mathf.RoundToInt(color.g * 255);
        var b = Mathf.RoundToInt(color.b * 255);
        var a = Mathf.RoundToInt(color.a * 255);
        var hexColor = $"#{r:X2}{g:X2}{b:X2}{a:X2}";
        return hexColor;
    }
    
    public static Color FromHex(string hexColor)
    {
        if (hexColor.StartsWith("#")) hexColor = hexColor.Replace("#", string.Empty);

        var red = 0;
        var green = 0;
        var blue = 0;
        var alpha = 255;

        switch (hexColor.Length)
        {
            case 8:
                red = int.Parse(hexColor.AsSpan(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                green = int.Parse(hexColor.AsSpan(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                blue = int.Parse(hexColor.AsSpan(4, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                alpha = int.Parse(hexColor.AsSpan(6, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                break;
            case 6:
                red = int.Parse(hexColor.AsSpan(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                green = int.Parse(hexColor.AsSpan(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                blue = int.Parse(hexColor.AsSpan(4, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                break;
            case 3:
                red = int.Parse(
                    hexColor[0] + hexColor[0].ToString(),
                    NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture);
                green = int.Parse(
                    hexColor[1] + hexColor[1].ToString(),
                    NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture);
                blue = int.Parse(
                    hexColor[2] + hexColor[2].ToString(),
                    NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture);
                break;
        }

        return new Color(red / 255f, green / 255f, blue / 255f, alpha / 255f);
    }
}