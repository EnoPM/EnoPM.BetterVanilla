using Hazel;

namespace BetterVanilla.Core.Extensions;

public static class MessageWriterPatches
{
    public static void SendImmediately(this MessageWriter writer)
    {
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}