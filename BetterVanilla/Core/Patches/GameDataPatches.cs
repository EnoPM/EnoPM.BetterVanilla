using BetterVanilla.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(GameData))]
internal static class GameDataPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(GameData.AddPlayerInfo))]
    private static void AddPlayerInfoPostfix(NetworkedPlayerInfo info)
    {
        info.RegisterFriendCode();
    }
}