using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models.Animation;

public partial class LoopNode : AnimationNode
{
    [ObservableProperty]
    private int _count = 2;

    public ObservableCollection<AnimationNode> Children { get; } = [];
}
