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

    private async void OnAddFrameClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: FrameListValue frameList }) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = frameList.Definition.Label,
            AllowMultiple = true,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("Images") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" } },
                FilePickerFileTypes.All
            }
        });

        foreach (var file in files)
            frameList.Frames.Add(File.ReadAllBytes(file.Path.LocalPath));
    }
}
