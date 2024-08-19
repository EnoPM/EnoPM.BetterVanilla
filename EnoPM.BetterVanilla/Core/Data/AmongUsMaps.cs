using EnoPM.BetterVanilla.Core.Attributes;

namespace EnoPM.BetterVanilla.Core.Data;

public enum AmongUsMaps : byte
{
    [DisplayAs("The Skeld")]
    Skeld = 0,
    
    [DisplayAs("Mira HQ")]
    MiraHq = 1,
    
    [DisplayAs("Polus")]
    Polus = 2,
    
    [DisplayAs("The Airship")]
    Airship = 4,
    
    [DisplayAs("The Fungle")]
    Fungle = 5
}