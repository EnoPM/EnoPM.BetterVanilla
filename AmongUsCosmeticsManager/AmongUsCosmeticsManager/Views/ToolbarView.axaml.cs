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

    private static readonly FilePickerFileType LegacyBundleFileType = new("Legacy Bundle")
    {
        Patterns = new[] { "*.bundle" }
    };

    private static readonly FilePickerFileType CosmeticsBundleFileType = new("Cosmetics Bundle")
    {
        Patterns = new[] { "*.bvcb" }
    };

    // === Open / Save project ===

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

    // === Import ===

    private async void OnImportLegacyBundleClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Importer un bundle legacy",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType> { LegacyBundleFileType }
        });

        if (files.Count > 0 && DataContext is MainViewModel vm)
            vm.ImportLegacyBundle(files[0].Path.LocalPath);
    }

    private async void OnImportCosmeticsBundleClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Importer un cosmetics bundle",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType> { CosmeticsBundleFileType }
        });

        if (files.Count > 0 && DataContext is MainViewModel vm)
            vm.ImportCosmeticsBundle(files[0].Path.LocalPath);
    }

    // === Export ===

    private async void OnExportCosmeticsBundleClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm || vm.SelectedBundle == null) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Exporter en Cosmetics Bundle",
            SuggestedFileName = vm.SelectedBundle.Name.Replace(' ', '_'),
            DefaultExtension = "bvcb",
            FileTypeChoices = new List<FilePickerFileType> { CosmeticsBundleFileType }
        });

        if (file != null)
            vm.ExportCosmeticsBundle(file.Path.LocalPath);
    }

    private async void OnExportLegacyBundleClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm || vm.SelectedBundle == null) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Exporter en Bundle Legacy",
            SuggestedFileName = vm.SelectedBundle.Name.Replace(' ', '_'),
            DefaultExtension = "bundle",
            FileTypeChoices = new List<FilePickerFileType> { LegacyBundleFileType }
        });

        if (file != null)
            vm.ExportLegacyBundle(file.Path.LocalPath);
    }
}
