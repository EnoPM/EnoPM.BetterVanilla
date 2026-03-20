using System;
using System.Collections.ObjectModel;
using System.Linq;
using AmongUsCosmeticsManager.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models;

public partial class CosmeticItem : ObservableObject
{
    public CosmeticTypeDefinition TypeDefinition { get; }

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _author = string.Empty;

    [ObservableProperty]
    private DateTime _lastModified = DateTime.Now;

    public ObservableCollection<PropertyValue> Properties { get; } = [];
    public ObservableCollection<ResourceValue> Resources { get; } = [];
    public ObservableCollection<FrameListValue> FrameLists { get; } = [];
    public ObservableCollection<ResourceGroup> ResourceGroups { get; } = [];

    public string TypeLabel => TypeDefinition.Label;

    public bool IsAdaptive => GetProperty("adaptive")?.BoolValue ?? false;

    public ResourceValue? MainResource => Resources.FirstOrDefault(r => r.Definition.Required)
                                          ?? Resources.FirstOrDefault();

    public ResourceValue? BackResource => GetResource("back");

    /// <summary>Flip resource data, or front data as fallback for the flip preview.</summary>
    public byte[]? FlipDisplayData => GetResource("flip")?.Data ?? MainResource?.Data;

    /// <summary>Back flip resource data, or back data as fallback.</summary>
    public byte[]? BackFlipDisplayData => GetResource("backFlip")?.Data ?? GetResource("back")?.Data;

    /// <summary>Climb resource data for the climb preview.</summary>
    public byte[]? ClimbDisplayData => GetResource("climb")?.Data;

    public CosmeticItem(CosmeticTypeDefinition typeDefinition)
    {
        TypeDefinition = typeDefinition;

        foreach (var prop in typeDefinition.Properties)
        {
            var pv = new PropertyValue(prop);
            if (prop.Id == "adaptive")
                pv.PropertyChanged += (_, _) => OnPropertyChanged(nameof(IsAdaptive));
            Properties.Add(pv);
        }

        foreach (var slot in typeDefinition.ResourceSlots)
        {
            var rv = new ResourceValue(slot);
            rv.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ResourceValue.Data))
                    NotifyPreviewProperties();
            };
            Resources.Add(rv);
        }

        foreach (var fl in typeDefinition.FrameLists)
            FrameLists.Add(new FrameListValue(fl));

        foreach (var slot in typeDefinition.ResourceSlots)
        {
            var resource = Resources.First(r => r.Definition.Id == slot.Id);
            var animation = slot.AnimationId != null
                ? FrameLists.FirstOrDefault(f => f.Definition.Id == slot.AnimationId)
                : null;
            ResourceGroups.Add(new ResourceGroup(resource, animation));
        }
    }

    public PropertyValue? GetProperty(string id) => Properties.FirstOrDefault(p => p.Definition.Id == id);
    public ResourceValue? GetResource(string id) => Resources.FirstOrDefault(r => r.Definition.Id == id);

    private void NotifyPreviewProperties()
    {
        OnPropertyChanged(nameof(FlipDisplayData));
        OnPropertyChanged(nameof(BackFlipDisplayData));
        OnPropertyChanged(nameof(ClimbDisplayData));
    }
}
