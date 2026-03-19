using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AmongUsCosmeticsManager.Models;
using AmongUsCosmeticsManager.Services;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace AmongUsCosmeticsManager.Converters;

public class AdaptiveCosmeticConverter : IMultiValueConverter
{
    public static readonly AdaptiveCosmeticConverter Instance = new();

    private static readonly SkiaSharp.SKColor VisorColor = new(149, 202, 220);

    // Simple cache: key = (dataHash, isAdaptive, colorName) → Bitmap
    private readonly Dictionary<(int, bool, string), WeakReference<Bitmap>> _cache = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 3) return null;
        if (values[0] is not byte[] { Length: > 0 } imageData) return null;

        var isAdaptive = values[1] is true;
        var playerColor = values[2] as PlayerColor;

        var cacheKey = (imageData.Length, isAdaptive, playerColor?.Name ?? "");

        if (_cache.TryGetValue(cacheKey, out var weakRef) && weakRef.TryGetTarget(out var cached))
            return cached;

        byte[] finalData;
        if (isAdaptive && playerColor != null)
            finalData = RecolorService.Recolor(imageData, playerColor.Body, VisorColor, playerColor.Shadow);
        else
            finalData = imageData;

        try
        {
            using var stream = new MemoryStream(finalData);
            var bitmap = new Bitmap(stream);

            _cache[cacheKey] = new WeakReference<Bitmap>(bitmap);
            return bitmap;
        }
        catch
        {
            return null;
        }
    }
}
