using Hazel;

namespace BetterVanilla.Core.Extensions;

public static class MessageWriterExtensions
{
    public static void SendImmediately(this MessageWriter writer)
    {
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}