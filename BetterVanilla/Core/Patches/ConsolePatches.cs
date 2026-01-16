using BetterVanilla.Components;
using BetterVanilla.Core.Extensions;
using HarmonyLib;
using UnityEngine;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(Console))]
internal static class ConsolePatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(Console.CanUse))]
    private static bool CanUsePrefix(Console __instance, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse, out float __result)
    {
        var num = float.MaxValue;
        var pc1 = pc.Object;
        var truePosition = pc1.GetTruePosition();
        var position = __instance.transform.position;
        var usable = __instance.As<IUsable>()!;
        var task = __instance.FindTask(pc1);

        couldUse = (!pc.IsDead || GameManager.Instance.LogicOptions.GetGhostsDoTasks() && !__instance.GhostsIgnored)
                   && pc1.CanMove
                   && pc.Role.CanUse(usable)
                   && (!__instance.onlySameRoom || __instance.InRoom(truePosition))
                   && (!__instance.onlyFromBelow || truePosition.y < position.y)
                   && task != null
                   && (!task.Is<NormalPlayerTask>() || !BetterVanillaManager.Instance.Menu.ButtonUi.autoTaskButton.IsRunning);
        canUse = couldUse;
        if (canUse)
        {
            num = Vector2.Distance(truePosition, __instance.transform.position);
            canUse &= num <= __instance.UsableDistance;
            if (__instance.checkWalls)
                canUse &= !PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShadowMask, false);
        }
        __result = num;
        return false;
    }
}