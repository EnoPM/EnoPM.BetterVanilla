using BetterVanilla.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(NetworkedPlayerInfo))]
internal static class NetworkedPlayerInfoPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(NetworkedPlayerInfo.Deserialize))]
    private static void DeserializePostfix(NetworkedPlayerInfo __instance)
    {
        __instance.RegisterFriendCode();
    }
}