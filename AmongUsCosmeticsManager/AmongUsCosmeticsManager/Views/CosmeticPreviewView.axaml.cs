using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AmongUsCosmeticsManager.Models;
using SkiaSharp;

namespace AmongUsCosmeticsManager.Views;

public partial class CosmeticPreviewView : UserControl
{
    public CosmeticPreviewView() => InitializeComponent();

    private async void OnBrowseResourceClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: ResourceValue resource }) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = resource.Definition.Label,
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("Images") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" } },
                FilePickerFileTypes.All
            }
        });

        if (files.Count > 0)
        {
            var path = files[0].Path.LocalPath;
            resource.FileName = Path.GetFileName(path);
            resource.Data = File.ReadAllBytes(path);
        }
    }

    private void OnClearResourceClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: ResourceValue resource }) return;
        resource.FileName = string.Empty;
        resource.Data = null;
    }

    private void OnGeneratePreviewClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: ResourceGroup group }) return;

        var vm = App.MainVM;
        var item = vm.SelectedItem;
        if (item == null) return;

        var frontData = GetResourceOrAnimationData(item, "front")
                        ?? GetResourceOrAnimationData(item, "resource");
        var backData = GetResourceOrAnimationData(item, "back");

        if (frontData == null && backData == null) return;

        var previewData = GeneratePreviewImage(frontData, backData);
        if (previewData == null) return;

        group.Resource.FileName = "preview.png";
        group.Resource.Data = previewData;
    }

    /// <summary>
    /// Returns the static resource data, or falls back to the first animation frame.
    /// </summary>
    private static byte[]? GetResourceOrAnimationData(CosmeticItem item, string slotId)
    {
        var group = item.ResourceGroups.FirstOrDefault(g => g.Resource.Definition.Id == slotId);
        return group?.DisplayData;
    }

    private static byte[]? GeneratePreviewImage(byte[]? frontData, byte[]? backData)
    {
        var frontBmp = frontData != null ? SKBitmap.Decode(frontData) : null;
        var backBmp = backData != null ? SKBitmap.Decode(backData) : null;

        if (frontBmp == null && backBmp == null) return null;

        try
        {
            // Compute the size needed to contain both sprites
            var maxW = Math.Max(frontBmp?.Width ?? 0, backBmp?.Width ?? 0);
            var maxH = Math.Max(frontBmp?.Height ?? 0, backBmp?.Height ?? 0);

            // Make the canvas square
            var size = Math.Max(maxW, maxH);
            if (size <= 0) return null;

            using var surface = SKSurface.Create(new SKImageInfo(size, size, SKColorType.Rgba8888, SKAlphaType.Premul));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            // Draw back first (behind), centered
            if (backBmp != null)
            {
                var x = (size - backBmp.Width) / 2f;
                var y = (size - backBmp.Height) / 2f;
                canvas.DrawBitmap(backBmp, x, y);
            }

            // Draw front on top, centered
            if (frontBmp != null)
            {
                var x = (size - frontBmp.Width) / 2f;
                var y = (size - frontBmp.Height) / 2f;
                canvas.DrawBitmap(frontBmp, x, y);
            }

            // Crop to non-transparent bounding box
            using var composited = surface.Snapshot();
            using var compositedBmp = SKBitmap.FromImage(composited);
            var bounds = FindOpaqueRegion(compositedBmp);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                using var fullData = composited.Encode(SKEncodedImageFormat.Png, 100);
                return fullData.ToArray();
            }

            // Extract the cropped region and make it square
            var cropSize = Math.Max(bounds.Width, bounds.Height);
            using var croppedSurface = SKSurface.Create(new SKImageInfo(cropSize, cropSize, SKColorType.Rgba8888, SKAlphaType.Premul));
            var croppedCanvas = croppedSurface.Canvas;
            croppedCanvas.Clear(SKColors.Transparent);

            var srcRect = SKRect.Create(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
            var dstRect = SKRect.Create(
                (cropSize - bounds.Width) / 2f,
                (cropSize - bounds.Height) / 2f,
                bounds.Width,
                bounds.Height);
            croppedCanvas.DrawImage(composited, srcRect, dstRect);

            using var croppedImage = croppedSurface.Snapshot();
            using var data = croppedImage.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }
        finally
        {
            frontBmp?.Dispose();
            backBmp?.Dispose();
        }
    }

    private static SKRectI FindOpaqueRegion(SKBitmap bitmap)
    {
        var minX = bitmap.Width;
        var minY = bitmap.Height;
        var maxX = 0;
        var maxY = 0;

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y).Alpha <= 0) continue;
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }
        }

        if (maxX < minX || maxY < minY)
            return SKRectI.Empty;

        return new SKRectI(minX, minY, maxX + 1, maxY + 1);
    }
}
