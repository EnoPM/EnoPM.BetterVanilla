using BetterVanilla.Components;
using UnityEngine;

namespace BetterVanilla.Core.Helpers;

public static class ConditionUtils
{
    public static bool AmDead()
    {
        return PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data && (PlayerControl.LocalPlayer.Data.IsDead || !BetterVanillaManager.Instance.LocalOptions.DisableAmDeadCheck.IsLocked() && BetterVanillaManager.Instance.LocalOptions.DisableAmDeadCheck.Value);
    }

    public static bool AmImpostor()
    {
        return PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data && PlayerControl.LocalPlayer.Data.Role && (PlayerControl.LocalPlayer.Data.Role.IsImpostor || !BetterVanillaManager.Instance.LocalOptions.DisableAmImpostorCheck.IsLocked() && BetterVanillaManager.Instance.LocalOptions.DisableAmImpostorCheck.Value);
    }

    public static bool AmAlive() => !AmDead();

    public static bool IsGameStarted()
    {
        return AmongUsClient.Instance && (AmongUsClient.Instance.IsGameStarted || TutorialManager.InstanceExists);
    }

    public static bool IsIncrementMultiplierKeyPressed()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
}