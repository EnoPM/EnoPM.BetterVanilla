using EnoPM.BetterVanilla.Attributes;

namespace EnoPM.BetterVanilla.Data;

public enum SettingTeamPreferences
{
    [DisplayAs("Crewmate team")]
    Crewmate,
    
    [DisplayAs("Impostor team")]
    Impostor,
    
    [DisplayAs("No preference")]
    Both,
}