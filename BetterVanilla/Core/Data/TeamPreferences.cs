using BetterVanilla.Core.Attributes;

namespace BetterVanilla.Core.Data;

public enum TeamPreferences
{
    [NamedField("Crewmate Team")]
    Crewmate,
    
    [NamedField("Impostor Team")]
    Impostor,
    
    [NamedField("No preference")]
    Both
}