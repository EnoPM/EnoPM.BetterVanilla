using System.Collections.ObjectModel;
using AmongUsCosmeticsManager.Models.Config;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models;

public partial class FrameListValue : ObservableObject
{
    public FrameListDefinition Definition { get; }
    public ObservableCollection<byte[]> Frames { get; } = [];

    public FrameListValue(FrameListDefinition definition)
    {
        Definition = definition;
    }
}
