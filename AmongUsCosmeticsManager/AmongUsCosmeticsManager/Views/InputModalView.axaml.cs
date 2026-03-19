using Avalonia.Controls;
using Avalonia.Input;
using AmongUsCosmeticsManager.ViewModels;

namespace AmongUsCosmeticsManager.Views;

public partial class InputModalView : UserControl
{
    public InputModalView()
    {
        InitializeComponent();

        ModalInputBox.KeyDown += (_, e) =>
        {
            if (DataContext is not MainViewModel vm) return;
            if (e.Key == Key.Enter) { vm.ConfirmModalCommand.Execute(null); e.Handled = true; }
            else if (e.Key == Key.Escape) { vm.CancelModalCommand.Execute(null); e.Handled = true; }
        };

        ModalInputBox.PropertyChanged += (_, e) =>
        {
            if (e.Property.Name == nameof(ModalInputBox.IsVisible) && ModalInputBox.IsVisible)
            {
                ModalInputBox.Focus();
                ModalInputBox.SelectAll();
            }
        };
    }
}
