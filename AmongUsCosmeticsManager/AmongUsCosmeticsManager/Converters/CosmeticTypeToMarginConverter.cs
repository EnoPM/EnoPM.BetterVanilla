using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AmongUsCosmeticsManager.Converters;

public class CosmeticTypeToLeftConverter : IValueConverter
{
    public static readonly CosmeticTypeToLeftConverter Instance = new();

    // Body pivot X = 81 (from 154 * 0.526)
    // Hat: pivot + 20 = 101
    // Visor: pivot + 17 = 98
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value as string) switch
        {
            "hat" => 101.0,
            "visor" => 98.0,
            "nameplate" => 77.0,
            _ => 81.0
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class CosmeticTypeToTopConverter : IValueConverter
{
    public static readonly CosmeticTypeToTopConverter Instance = new();

    // Body pivot Y = 106 (from 202 * 0.525)
    // Hat: pivot - 75 = 31
    // Visor: pivot - 75 = 31
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value as string) switch
        {
            "hat" => 31.0,
            "visor" => 31.0,
            "nameplate" => 210.0,
            _ => 50.0
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
