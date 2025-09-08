using System;
using System.Collections.Generic;
using BetterVanilla.Components;
using BetterVanilla.Core.Extensions;
using Hazel;
using InnerNet;

namespace BetterVanilla.Core;

public sealed class CheatersManager
{
    public readonly Dictionary<byte, string> SickoUsers = [];
    public readonly Dictionary<byte, string> AumUsers = [];

    public bool IsCheating(BetterPlayerControl player)
    {
        return IsCheating(player.Player);
    }
    
    public bool IsCheating(PlayerControl? player)
    {
        if (player == null) return false;
        var id = player.PlayerId;
        return SickoUsers.ContainsKey(id) || AumUsers.ContainsKey(id);
    }

    public void HandleRpc(PlayerControl sender, byte callId, MessageReader reader)
    {
        HandleSicko(sender, callId, reader);
        HandleAum(sender, callId, reader);
    }
    
    private void HandleSicko(PlayerControl player, byte callId, MessageReader reader)
    {
        if (callId != 164 || reader.BytesRemaining != 0) return;
        if (IsCheating(player)) return;
        player.ReportPlayer(ReportReasons.Cheating_Hacking);
        SickoUsers.Add(player.PlayerId, player.FriendCode);
    }

    private void HandleAum(PlayerControl player, byte callId, MessageReader reader)
    {
        if (callId != 85 && callId != 101) return;
        if (callId == 101)
        {
            try
            {
                var nameString = reader.ReadString();
                if (player.Data.PlayerName != nameString)
                {
                    throw new Exception();
                }
            }
            catch
            {
                return;
            }
        }
        if (IsCheating(player)) return;
        player.ReportPlayer(ReportReasons.Cheating_Hacking);
        AumUsers.Add(player.PlayerId, player.FriendCode);
    }
}