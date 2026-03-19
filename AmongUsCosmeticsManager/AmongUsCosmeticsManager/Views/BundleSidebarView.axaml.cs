using Avalonia.Controls;
using Avalonia.Interactivity;
using AmongUsCosmeticsManager.Models;
using AmongUsCosmeticsManager.ViewModels;

namespace AmongUsCosmeticsManager.Views;

public partial class BundleSidebarView : UserControl
{
    public BundleSidebarView()
    {
        InitializeComponent();

        RenameBundleMenuItem.Click += (_, _) =>
        {
            if (DataContext is MainViewModel vm)
                vm.ShowRenameBundleModalCommand.Execute(null);
        };

        DeleteBundleMenuItem.Click += (_, _) =>
        {
            if (DataContext is MainViewModel vm && vm.SelectedBundle != null)
                vm.DeleteBundle(vm.SelectedBundle);
        };
    }

    private void OnAddCosmeticClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: CosmeticSection section } && DataContext is MainViewModel vm)
            vm.AddCosmetic(section);
    }
}
