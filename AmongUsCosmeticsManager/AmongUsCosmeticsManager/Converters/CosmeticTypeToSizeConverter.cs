using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AmongUsCosmeticsManager.Converters;

public class CosmeticTypeToSizeConverter : IValueConverter
{
    public static readonly CosmeticTypeToSizeConverter Instance = new();

    // Sizes relative to crewmate body (154x202)
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value as string) switch
        {
            "hat" => 160.0,
            "visor" => 110.0,
            "nameplate" => 180.0,
            _ => 140.0
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
