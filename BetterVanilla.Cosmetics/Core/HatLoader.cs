using BetterVanilla.Cosmetics.Api.Serialization;

namespace BetterVanilla.Cosmetics.Core;

public class HatLoader
{
    public SerializedHat Hat { get; }

    public HatLoader(SerializedHat hat)
    {
        Hat = hat;
    }
}