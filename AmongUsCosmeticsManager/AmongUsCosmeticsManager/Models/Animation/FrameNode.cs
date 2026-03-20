using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models.Animation;

public partial class FrameNode : AnimationNode
{
    [ObservableProperty]
    private byte[] _data = [];

    [ObservableProperty]
    private int? _durationMs;

    [ObservableProperty]
    private int _effectiveDurationMs = 100;

    private bool _internalUpdate;

    partial void OnDurationMsChanged(int? value)
    {
        if (!_internalUpdate && value.HasValue)
        {
            _internalUpdate = true;
            EffectiveDurationMs = value.Value;
            _internalUpdate = false;
        }
    }

    partial void OnEffectiveDurationMsChanged(int value)
    {
        // Only store as custom override when edited by the user (not from FPS update)
        if (!_internalUpdate)
            DurationMs = value;
    }

    /// <summary>
    /// Called by FrameListValue when DefaultFps changes. Updates display without creating an override.
    /// </summary>
    internal void SetEffectiveFromFps(int durationMs)
    {
        if (DurationMs.HasValue) return; // has custom override, don't touch

        _internalUpdate = true;
        EffectiveDurationMs = durationMs;
        _internalUpdate = false;
    }
}
