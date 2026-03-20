using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models;

public partial class ResourceGroup : ObservableObject
{
    public ResourceValue Resource { get; }
    public FrameListValue? Animation { get; }

    public string Label => Resource.Definition.Label;
    public bool HasAnimation => Animation != null;

    public byte[]? DisplayData => Resource.HasData
        ? Resource.Data
        : Animation?.FirstFrameData;

    public bool HasDisplayData => DisplayData is { Length: > 0 };

    public ResourceGroup(ResourceValue resource, FrameListValue? animation)
    {
        Resource = resource;
        Animation = animation;

        resource.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(ResourceValue.Data) or nameof(ResourceValue.HasData))
                NotifyDisplayChanged();
        };

        if (animation != null)
        {
            animation.Nodes.CollectionChanged += (_, _) => NotifyDisplayChanged();
        }
    }

    private void NotifyDisplayChanged()
    {
        OnPropertyChanged(nameof(DisplayData));
        OnPropertyChanged(nameof(HasDisplayData));
    }
}
