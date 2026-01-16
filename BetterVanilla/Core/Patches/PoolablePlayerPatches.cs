using BetterVanilla.Components;
using BetterVanilla.Core.Extensions;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(PoolablePlayer))]
internal static class PoolablePlayerPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(PoolablePlayer.UpdateFromPlayerData))]
    private static void UpdateFromPlayerDataPostfix(PoolablePlayer __instance, NetworkedPlayerInfo pData)
    {
        UpdatePoolablePlayerMaterials(__instance, pData);
    }
    
    [HarmonyPostfix, HarmonyPatch(nameof(PoolablePlayer.UpdateFromEitherPlayerDataOrCache))]
    private static void UpdateFromEitherPlayerDataOrCachePostfix(PoolablePlayer __instance, NetworkedPlayerInfo pData)
    {
        UpdatePoolablePlayerMaterials(__instance, pData);
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PoolablePlayer.UpdateFromLocalPlayer))]
    [HarmonyPatch(nameof(PoolablePlayer.UpdateFromDataManager), typeof(PlayerMaterial.MaskType))]
    [HarmonyPatch(nameof(PoolablePlayer.UpdateFromDataManager), typeof(PlayerMaterial.MaskType), typeof(int))]
    private static void UpdateFromLocalPlayerPostfix(PoolablePlayer __instance)
    {
        __instance.SetLocalVisorColor();
    }

    private static void UpdatePoolablePlayerMaterials(PoolablePlayer poolable, NetworkedPlayerInfo info)
    {
        var player = BetterVanillaManager.Instance.GetPlayerById(info.PlayerId);
        if (player != null && player.AmSponsor)
        {
            poolable.SetVisorColor(player.GetVisorColor());
        }
    }
}