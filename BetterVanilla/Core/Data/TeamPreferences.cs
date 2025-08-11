using BetterVanilla.Core.Attributes;

namespace BetterVanilla.Core.Data;

public enum TeamPreferences
{
    [Name("Crewmate Team")]
    Crewmate,
    
    [Name("Impostor Team")]
    Impostor,
    
    [Name("No preference")]
    Both
}