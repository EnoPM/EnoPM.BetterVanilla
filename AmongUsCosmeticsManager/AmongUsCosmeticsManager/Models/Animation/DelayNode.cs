using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models.Animation;

public partial class DelayNode : AnimationNode
{
    [ObservableProperty]
    private int _durationMs = 500;
}
