using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options.Core.Local;

public sealed class BoolLocalOption(string key, string title, bool defaultValue) : BoolSerializableOption(key, title, defaultValue)
{
    
}