using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(MapRoom))]
internal static class MapRoomPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(MapRoom.Start))]
    private static void StartPostfix(MapRoom __instance)
    {
        if (__instance.special)
        {
            var pos = __instance.special.transform.localPosition;
            pos.z = -3f;
            __instance.special.transform.localPosition = pos;
        }
        
        if (__instance.door)
        {
            var pos = __instance.door.transform.localPosition;
            pos.z = -3f;
            __instance.door.transform.localPosition = pos;
        }
    }
}