using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AmongUsCosmeticsManager.Models;

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
}
