using BetterVanilla.Options.Core.Serialization;

namespace BetterVanilla.Options.Core.Local;

public sealed class TextLocalOption(string key, string title, string defaultValue) : TextSerializableOption(key, title, defaultValue)
{
    
}