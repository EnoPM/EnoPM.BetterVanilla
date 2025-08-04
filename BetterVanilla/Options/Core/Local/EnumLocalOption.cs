using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options.Core.Local;

public sealed class EnumLocalOption(string key, string title, object defaultValue) : EnumSerializableOption(key, title, defaultValue)
{
    
}