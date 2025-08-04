using BetterVanilla.Options.Core.Serialization;
using UnityEngine;

namespace BetterVanilla.Options.Core.Local;

public sealed class ColorLocalOption(string key, string title, Color defaultValue) : ColorSerializableOption(key, title, defaultValue)
{
    
}