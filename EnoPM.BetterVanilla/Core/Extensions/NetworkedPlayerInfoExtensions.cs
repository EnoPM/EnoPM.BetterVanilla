using System;
using UnityEngine;

namespace EnoPM.BetterVanilla.Core.Extensions;

internal static class NetworkedPlayerInfoExtensions
{
    internal static string GenerateModdedText(this NetworkedPlayerInfo info)
    {
        var role = info.Role;
        var amDead = Utils.AmDead;
        var shouldShowRolesAndTasks = (bool)ModSettings.Local.DisplayRolesAndTasksAfterDeath;
        var isHost = Utils.IsHost(info.Object);
        var isDisconnected = info.Disconnected;
        var isCheater = info.Object && info.Object.IsCheater();
        var text = "<size=70%>";
        if (isCheater)
        {
            text += Utils.Cs(ModConfigs.CheaterColor, "Cheater");
            if (isHost || isDisconnected)
            {
                text += " - ";
            }
        }
        if (isHost)
        {
            text += Utils.Cs(ModConfigs.HostColor, "Host");
        }
        else if (isDisconnected)
        {
            text += Utils.Cs(ModConfigs.ImpostorColor, "Disconnected");
        }
        if (shouldShowRolesAndTasks && role && Utils.IsGameStarted && (amDead || Utils.IsLocalPlayer(info.PlayerId)))
        {
            if (isHost || isDisconnected)
            {
                text += " - ";
            }
            if (role.IsImpostor)
            {
                text += Utils.Cs(Palette.ImpostorRed, DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Impostor));
            }
            else
            {
                var (done, total) = Utils.GetTasksCount(info);
                var half = Mathf.RoundToInt(total / 2f);
                var color = ModConfigs.NoTasksDoneColor;
                if (done > 0 && done < half)
                {
                    color = ModConfigs.LessThanHalfTasksDoneColor;
                }
                else if (done >= total)
                {
                    color = ModConfigs.AllTasksDoneColor;
                }
                else if (done > 0 && done >= half)
                {
                    color = ModConfigs.MoreThanHalfTasksDoneColor;
                }
                text += Utils.Cs(color, $"{done}/{total}");
            }
        }
        text += "</size>";

        return text;
    }
}