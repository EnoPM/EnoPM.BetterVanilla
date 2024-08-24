using System;
using System.Collections.Generic;
using EnoPM.BetterVanilla.Core.Extensions;
using Hazel;
using InnerNet;

namespace EnoPM.BetterVanilla.Core;

public static class CheatsManager
{
    public static bool Enabled { get; set; } = true;

    public static readonly Dictionary<string, string> SickoUsers = [];
    public static readonly Dictionary<string, string> AumUsers = [];
    private static readonly HashSet<byte> TrustedRpcCallIds = [];

    static CheatsManager()
    {
        foreach (var enumValue in Enum.GetValues<RpcCalls>())
        {
            TrustedRpcCallIds.Add((byte)enumValue);
        }
        TrustedRpcCallIds.Add(CustomRpcManager.RpcId);
    }

    public static bool IsCheating(PlayerControl player)
    {
        var puId = player.GetPuId();
        return SickoUsers.ContainsKey(puId) || AumUsers.ContainsKey(puId);
    }

    private static void HandleSicko(PlayerControl player, byte callId, MessageReader reader)
    {
        if (callId != 164 || reader.BytesRemaining != 0) return;
        var puId = player.GetPuId();
        if (SickoUsers.ContainsKey(puId)) return;
        player.ReportPlayer(ReportReasons.Cheating_Hacking);
        SickoUsers.Add(puId, player.FriendCode);
    }

    private static void HandleAum(PlayerControl player, byte callId, MessageReader reader)
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
        var puId = player.GetPuId();
        if (AumUsers.ContainsKey(puId)) return;
        player.ReportPlayer(ReportReasons.Cheating_Hacking);
        AumUsers.Add(puId, player.FriendCode);
    }

    public static void HandleRPCBeforeCheck(PlayerControl player, byte callId, MessageReader oldReader)
    {
        if (!Enabled || !player || !PlayerControl.LocalPlayer || player == PlayerControl.LocalPlayer) return;
        var messageReader = MessageReader.Get(oldReader);
        HandleSicko(player, callId, messageReader);
        HandleAum(player, callId, messageReader);
    }

    public static bool ShouldCancelRpc(PlayerControl player, byte callId, MessageReader reader)
    {
        if (!CheckCancelRpc(player, callId, reader))
        {
            Plugin.Logger.LogWarning($"[EnoAC] Rpc canceled: {Enum.GetName((RpcCalls)callId)} - {callId}");
            return true;
        }
        if (AmongUsClient.Instance.AmHost && !CheckRpcAsHost(player, callId, reader))
        {
            Plugin.Logger.LogWarning($"[EnoAC] Rpc canceled as host : {Enum.GetName((RpcCalls)callId)} - {callId}");
            return true;
        }
        
        CheckRpc(player, callId, reader);
        return false;
    }

    private static void LogInvalidActionRpc(PlayerControl player, byte callId, string msg)
    {
        Plugin.Logger.LogWarning($"[EnoAC] Invalid action in rpc {Enum.GetName((RpcCalls)callId)}: {msg}");
    }

    private static bool CheckMurderPlayerRpc(PlayerControl player, byte callId, MessageReader reader)
    {
        var target = reader.ReadNetObject<PlayerControl>();
        if (target && !target.IsAlive() && target == PlayerControl.LocalPlayer)
        {
            LogInvalidActionRpc(player, callId, "target is not alive");
            return false;
        }
        return true;
    }

    private static bool CheckCancelRpc(PlayerControl player, byte callId, MessageReader reader)
    {
        try
        {
            var self = MessageReader.Get(reader);
            if (!player || !PlayerControl.LocalPlayer || player == PlayerControl.LocalPlayer || self == null) return true;
            if (callId == (byte)RpcCalls.MurderPlayer && self.BytesRemaining > 0)
            {
                if (!CheckMurderPlayerRpc(player, callId, reader))
                {
                    
                }
            }
        }
        catch(Exception ex)
        {
            Plugin.Logger.LogMessage($"[EnoAntiCheat] {ex.ToString()}");
        }
        return false;
    }

    private static bool CheckRpcAsHost(PlayerControl player, byte callId, MessageReader reader)
    {
        return false;
    }

    private static void CheckRpc(PlayerControl player, byte callId, MessageReader reader)
    {
        
    }
}