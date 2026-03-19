using AmongUsCosmeticsManager.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models;

public partial class PropertyValue : ObservableObject
{
    public PropertyDefinition Definition { get; }

    [ObservableProperty]
    private object? _value;

    public PropertyValue(PropertyDefinition definition)
    {
        Definition = definition;
        Value = definition.Default;
    }

    public bool BoolValue
    {
        get => Value is true or "True" or "true";
        set { Value = value; OnPropertyChanged(); }
    }

    public string StringValue
    {
        get => Value as string ?? string.Empty;
        set { Value = value; OnPropertyChanged(); }
    }

    partial void OnValueChanged(object? value)
    {
        OnPropertyChanged(nameof(BoolValue));
        OnPropertyChanged(nameof(StringValue));
    }
}
