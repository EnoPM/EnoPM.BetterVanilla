using System;
using EnoPM.BetterVanilla.Core;
using EnoPM.BetterVanilla.Patches;
using UnityEngine;

namespace EnoPM.BetterVanilla.Extensions;

internal static class PlayerInfoExtensions
{
    internal static string GenerateModdedText(this GameData.PlayerInfo info)
    {
        var role = info.Role;
        var amDead = Utils.AmDead;
        var isHost = Utils.IsHost(info.Object);
        var isDisconnected = info.Disconnected;
        var isCheater = info.Object && PlayerControlPatches.CheaterOwnerIds.Contains(info.Object.OwnerId);
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
        if (role != null && Utils.IsGameStarted && (amDead || Utils.IsLocalPlayer(info.PlayerId)))
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