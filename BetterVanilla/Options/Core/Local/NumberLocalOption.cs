using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options.Core.Local;

public sealed class NumberLocalOption(
    string key,
    string title,
    float defaultValue,
    float incrementValue,
    float minValue,
    float maxValue,
    string valuePrefix = "",
    string valueSuffix = ""
)
    : NumberSerializableOption(
        key,
        title,
        defaultValue,
        incrementValue,
        minValue,
        maxValue,
        valuePrefix,
        valueSuffix
    )
{
    
}