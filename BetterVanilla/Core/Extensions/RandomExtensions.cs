using System;

namespace BetterVanilla.Core.Extensions;

public static class RandomExtensions
{
    public static float Next(this Random random, float minValue, float maxValue)
    {
        return (float) (random.NextDouble() * (maxValue - minValue) + minValue);
    }
}