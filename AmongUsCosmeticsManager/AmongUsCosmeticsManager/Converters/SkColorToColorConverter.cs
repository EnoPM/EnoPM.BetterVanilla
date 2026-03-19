using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using SkiaSharp;

namespace AmongUsCosmeticsManager.Converters;

public class SkColorToColorConverter : IValueConverter
{
    public static readonly SkColorToColorConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SKColor sk)
            return Color.FromArgb(sk.Alpha, sk.Red, sk.Green, sk.Blue);
        return Colors.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
