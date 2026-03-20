using CommunityToolkit.Mvvm.ComponentModel;

namespace AmongUsCosmeticsManager.Models.Animation;

public partial class FrameNode : AnimationNode
{
    [ObservableProperty]
    private byte[] _data = [];

    [ObservableProperty]
    private int? _durationMs;

    [ObservableProperty]
    private int _fpsDefaultDurationMs = 100;

    /// <summary>
    /// The effective duration used by the playback engine.
    /// </summary>
    public int EffectiveDurationMs => DurationMs ?? FpsDefaultDurationMs;

    /// <summary>
    /// True when DurationMs is null (using FPS-calculated default).
    /// </summary>
    public bool IsUsingFpsDefault => !DurationMs.HasValue;

    /// <summary>
    /// String property for TextBox binding.
    /// Always shows the effective value. Clearing the field removes the custom override.
    /// </summary>
    public string DurationDisplay
    {
        get => EffectiveDurationMs.ToString();
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                DurationMs = null;
            else if (int.TryParse(value, out var ms) && ms > 0)
                DurationMs = ms;
        }
    }

    partial void OnDurationMsChanged(int? value)
    {
        OnPropertyChanged(nameof(EffectiveDurationMs));
        OnPropertyChanged(nameof(DurationDisplay));
        OnPropertyChanged(nameof(IsUsingFpsDefault));
    }

    partial void OnFpsDefaultDurationMsChanged(int value)
    {
        if (!DurationMs.HasValue)
        {
            OnPropertyChanged(nameof(EffectiveDurationMs));
            OnPropertyChanged(nameof(DurationDisplay));
        }
    }

    /// <summary>
    /// Called by FrameListValue when DefaultFps changes.
    /// </summary>
    internal void SetEffectiveFromFps(int durationMs)
    {
        FpsDefaultDurationMs = durationMs;
    }
}
