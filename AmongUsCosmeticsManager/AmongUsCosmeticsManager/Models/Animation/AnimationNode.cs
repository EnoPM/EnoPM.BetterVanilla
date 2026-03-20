using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models.Animation;

public abstract partial class AnimationNode : ObservableObject
{
    [ObservableProperty]
    private bool _isPlayheadHere;
}
