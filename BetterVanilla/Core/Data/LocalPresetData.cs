using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BetterVanilla.Core.Data;

public sealed class LocalPresetData
{
    [JsonPropertyName("bool_store")]
    public Dictionary<string, bool> BoolStore { get; set; } = [];

    [JsonPropertyName("string_store")]
    public Dictionary<string, string> StringStore { get; set; } = [];

    [JsonPropertyName("int_store")]
    public Dictionary<string, int> IntStore { get; set; } = [];

    [JsonPropertyName("float_store")]
    public Dictionary<string, float> FloatStore { get; set; } = [];

    public bool GetValueOrDefault(string key, bool defaultValue)
    {
        return BoolStore.GetValueOrDefault(key, defaultValue);
    }

    public string GetValueOrDefault(string key, string defaultValue)
    {
        return StringStore.GetValueOrDefault(key, defaultValue);
    }

    public int GetValueOrDefault(string key, int defaultValue)
    {
        return IntStore.GetValueOrDefault(key, defaultValue);
    }

    public float GetValueOrDefault(string key, float defaultValue)
    {
        return FloatStore.GetValueOrDefault(key, defaultValue);
    }
    
    public TEnum GetValueOrDefault<TEnum>(string key, TEnum defaultValue) where TEnum : struct
    {
        return Enum.Parse<TEnum>(StringStore.GetValueOrDefault(key, defaultValue.ToString()));
    }
}