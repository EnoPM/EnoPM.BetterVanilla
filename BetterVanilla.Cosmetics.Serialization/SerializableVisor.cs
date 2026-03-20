using BetterVanilla.Cosmetics.Serialization.Core;
using BetterVanilla.Cosmetics.Serialization.Interfaces;

namespace BetterVanilla.Cosmetics.Serialization;

public sealed class SerializableVisor : SerializableCosmeticBase, ICustomVisor<SerializableSprite, SerializableFrameAnimation>
{
    public bool IsBounce { get; set; }
    
    public bool DisableVisors { get; set; }
    
    public bool IsBehindHat { get; set; }

    public SerializableSprite? Front { get; set; }
    public SerializableFrameAnimation? FrontAnimation { get; set; }

    public SerializableSprite? Left { get; set; }
    public SerializableFrameAnimation? LeftAnimation { get; set; }

    public SerializableSprite? Floor { get; set; }
    public SerializableFrameAnimation? FloorAnimation { get; set; }
}