using Hazel;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class MessageReaderExtensions
{
    public static Color ReadColor(this MessageReader reader)
    {
        var r = reader.ReadSingle();
        var g = reader.ReadSingle();
        var b = reader.ReadSingle();
        var a = reader.ReadSingle();
        var color = new Color(r, g, b, a);
        return color;
    }
}