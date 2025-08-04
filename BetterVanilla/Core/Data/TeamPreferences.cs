using BetterVanilla.Core.Attributes;

namespace BetterVanilla.Core.Data;

public enum TeamPreferences
{
    [NamedField("Crewmate Team")]
    [Name("Crewmate Team")]
    Crewmate,
    
    [NamedField("Impostor Team")]
    [Name("Impostor Team")]
    Impostor,
    
    [NamedField("No preference")]
    [Name("No preference")]
    Both
}