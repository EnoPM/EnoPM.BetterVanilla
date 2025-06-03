using BetterVanilla.Core.Attributes;

namespace BetterVanilla.Core.Data;

public enum DoorOpenDelay
{
    [NamedField("0.1s")]
    Ms1,
    
    [NamedField("0.2s")]
    Ms2,
    
    [NamedField("0.3s")]
    Ms3,
    
    [NamedField("0.4s")]
    Ms4,
    
    [NamedField("0.5s")]
    Ms5,
}