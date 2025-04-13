using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using BetterVanilla.Core.Extensions;

namespace BetterVanilla.Core.Data;

public sealed class AutoTaskData
{
    [JsonPropertyName("duration")]
    public float? Duration { get; set; }
    [JsonPropertyName("min_duration")]
    public float? MinDuration { get; set; }
    [JsonPropertyName("max_duration")]
    public float? MaxDuration { get; set; }
    [JsonPropertyName("step_delay")]
    public float? StepDelay { get; set; }
    
    private static readonly Random RandomGenerator = new();
    
    public float GetDuration()
    {
        if (Duration.HasValue)
        {
            return Duration.Value;
        }
        if (MinDuration.HasValue && MaxDuration.HasValue)
        {
            return RandomGenerator.Next(MinDuration.Value, MaxDuration.Value);
        }

        throw new Exception($"Wrong AutoTaskData: {JsonSerializer.Serialize(this)}");
    }
}