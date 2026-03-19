using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AmongUsCosmeticsManager.ViewModels;

namespace AmongUsCosmeticsManager.Views;

public partial class ToolbarView : UserControl
{
    public ToolbarView() => InitializeComponent();

    private static readonly FilePickerFileType ProjectFileType = new("BV Cosmetics Project")
    {
        Patterns = new[] { "*.bvcp" }
    };

    private static readonly FilePickerFileType BundleFileType = new("BV Compiled Bundle")
    {
        Patterns = new[] { "*.bundle" }
    };

    private async void OnOpenProjectClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Ouvrir un projet",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType> { ProjectFileType }
        });

        if (files.Count > 0 && DataContext is MainViewModel vm)
            vm.OpenProject(files[0].Path.LocalPath);
    }

    private async void OnSaveProjectClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;

        if (!vm.HasProjectFile)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Enregistrer le projet",
                DefaultExtension = "bvcp",
                FileTypeChoices = new List<FilePickerFileType> { ProjectFileType }
            });

            if (file == null) return;
            vm.SetProjectFile(file.Path.LocalPath);
        }

        vm.SaveProjectCommand.Execute(null);
    }

    private async void OnImportBundleClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Importer un bundle compile",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType> { BundleFileType }
        });

        if (files.Count > 0 && DataContext is MainViewModel vm)
            vm.ImportBundle(files[0].Path.LocalPath);
    }
}
