using Hazel;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class MessageWriterExtensions
{
    public static void SendImmediately(this MessageWriter writer)
    {
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
    
    public static void Write(this MessageWriter writer, Color color)
    {
        writer.Write(color.r);
        writer.Write(color.g);
        writer.Write(color.b);
        writer.Write(color.a);
    }
}