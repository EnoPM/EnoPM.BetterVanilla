using EnoPM.BetterVanilla.Core.Attributes;

namespace EnoPM.BetterVanilla.Core.Data;

public enum SettingTeamPreferences
{
    [DisplayAs("Crewmate Team")]
    Crewmate,
    
    [DisplayAs("Impostor Team")]
    Impostor,
    
    [DisplayAs("No Preference")]
    Both,
}