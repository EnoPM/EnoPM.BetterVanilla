using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class NumberOptionExtensions
{
    private const int IncrementMultiplier = 10;
    
    public static void BetterIncrease(this NumberOption option)
    {
        if (Mathf.Approximately(option.Value, option.ValidRange.max)) return;
        var multiplier = option.ValidRange.max - option.ValidRange.min >= IncrementMultiplier * option.Increment && LocalConditions.IsIncrementMultiplierKeyPressed() ? IncrementMultiplier : 1;
        option.Value = option.ValidRange.Clamp(option.Value + option.Increment * multiplier);
        option.UpdateValue();
        option.OnValueChanged.Invoke(option);
        option.AdjustButtonsActiveState();
    }

    public static void BetterDecrease(this NumberOption option)
    {
        if (Mathf.Approximately(option.Value, option.ValidRange.min)) return;
        var multiplier = option.ValidRange.max - option.ValidRange.min >= IncrementMultiplier * option.Increment && LocalConditions.IsIncrementMultiplierKeyPressed() ? IncrementMultiplier : 1;
        option.Value = option.ValidRange.Clamp(option.Value - option.Increment * multiplier);
        option.UpdateValue();
        option.OnValueChanged.Invoke(option);
        option.AdjustButtonsActiveState();
    }
}