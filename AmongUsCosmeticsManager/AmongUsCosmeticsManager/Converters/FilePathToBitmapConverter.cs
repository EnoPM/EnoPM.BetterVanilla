using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace AmongUsCosmeticsManager.Converters;

public class BytesToBitmapConverter : IValueConverter
{
    public static readonly BytesToBitmapConverter Instance = new();

    private readonly Dictionary<int, WeakReference<Bitmap>> _cache = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not byte[] { Length: > 0 } data)
            return null;

        var key = data.Length;
        if (_cache.TryGetValue(key, out var weakRef) && weakRef.TryGetTarget(out var cached))
            return cached;

        try
        {
            using var stream = new MemoryStream(data);
            var bitmap = new Bitmap(stream);
            _cache[key] = new WeakReference<Bitmap>(bitmap);
            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
