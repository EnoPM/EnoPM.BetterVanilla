using BetterVanilla.Components;
using BetterVanilla.Options;
using HarmonyLib;
using Hazel;
using Il2CppSystem;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(ReactorSystemType))]
internal static class ReactorSystemTypePatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(ReactorSystemType.UpdateSystem))]
    private static bool UpdateSystemPrefix(ReactorSystemType __instance, PlayerControl player, MessageReader msgReader)
    {
        var self = msgReader.ReadByte();
        var num = self & 3;
        if (self == 128 && !__instance.IsActive)
        {
            __instance.Countdown = ShipStatus.Instance.Type != ShipStatus.MapType.Pb
                ? __instance.ReactorDuration
                : HostOptions.Default.PolusReactorCountdown.Value;
            __instance.UserConsolePairs.Clear();
        }
        else if (self == 16)
        {
            __instance.Countdown = 10000f;
        }
        else if (self.HasAnyBit(64))
        {
            __instance.UserConsolePairs.Add(new Tuple<byte, byte>(player.PlayerId, (byte) num));
            if (__instance.UserCount >= 2)
            {
                __instance.Countdown = 10000f;
            }
        }
        else if (self.HasAnyBit(32))
        {
            __instance.UserConsolePairs.Remove(new Tuple<byte, byte>(player.PlayerId, (byte) num));
        }

        __instance.IsDirty = true;

        return false;
    }
}