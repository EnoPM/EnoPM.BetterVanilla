using Avalonia.Controls;
using AmongUsCosmeticsManager.ViewModels;

namespace AmongUsCosmeticsManager.Views;

public partial class PropertiesPanelView : UserControl
{
    public PropertiesPanelView()
    {
        InitializeComponent();

        DeleteItemButton.Click += (_, _) =>
        {
            if (DataContext is MainViewModel vm && vm.SelectedItem != null)
                vm.DeleteItemCommand.Execute(vm.SelectedItem);
        };
    }
}
