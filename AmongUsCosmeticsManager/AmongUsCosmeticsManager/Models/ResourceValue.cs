using AmongUsCosmeticsManager.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models;

public partial class ResourceValue : ObservableObject
{
    public ResourceSlotDefinition Definition { get; }

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private byte[]? _data;

    public bool HasData => Data != null && Data.Length > 0;

    public ResourceValue(ResourceSlotDefinition definition)
    {
        Definition = definition;
    }

    partial void OnDataChanged(byte[]? value)
    {
        OnPropertyChanged(nameof(HasData));
    }
}
