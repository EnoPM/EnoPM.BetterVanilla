using System.Collections.Generic;
using EnoPM.BetterVanilla.Core.Data;
using EnoPM.BetterVanilla.Core.Settings;
using EnoPM.BetterVanilla.Patches;
using Hazel;

namespace EnoPM.BetterVanilla.Core;

public static class CustomRpcManager
{
    public const byte RpcId = 252;
    
    private enum Rpc : uint
    {
        BetterVanillaHandshake,
        TeamPreference,
        ForcedTeamAssignment,
        PrivateChatMessage,
        ShareHostToClientSettingChange,
        BulkShareHostToClientSettingChange,
    }

    public static void CustomSpawnHandshake(PlayerControl spawnedPlayer)
    {
        if (AmongUsClient.Instance.AmHost && PlayerControl.LocalPlayer)
        {
            PlayerControl.LocalPlayer.RpcShareAllHostToClientSettings();
        }
    }

    public static void CustomOwnerSpawnHandshake(PlayerControl localPlayer)
    {
        localPlayer.RpcSetTeamAssignmentPreference(ModSettings.Local.TeamPreference);
        if (!ModSettings.Local.ForcedTeamAssignment.IsLocked() && ModSettings.Local.ForcedTeamAssignment != SettingTeamPreferences.Both)
        {
            localPlayer.RpcSetForcedTeamAssignment(ModSettings.Local.ForcedTeamAssignment);
        }
    }
    
    public static void HandleCustomRpc(this PlayerControl sender, MessageReader reader)
    {
        var callId = reader.ReadUInt32();
        switch ((Rpc)callId)
        {
            case Rpc.TeamPreference:
                sender.SetTeamAssignmentPreference((SettingTeamPreferences)reader.ReadByte());
                break;
            case Rpc.ForcedTeamAssignment:
                sender.SetForcedTeamAssignment((SettingTeamPreferences)reader.ReadByte());
                break;
            case Rpc.PrivateChatMessage:
                ChatControllerPatches.HandlePrivateMessageRpc(sender, reader);
                break;
            case Rpc.ShareHostToClientSettingChange:
                CustomSetting.HandleRpcShareHostToClientSettingChange(sender, reader);
                break;
            case Rpc.BulkShareHostToClientSettingChange:
                CustomSetting.HandleRpcBulkShareHostToClientSettingChange(sender, reader);
                break;
            default:
                Plugin.Logger.LogWarning($"Unknown custom rpc received: {callId}");
                break;
        }
    }

    public static void RpcShareAllHostToClientSettings(this PlayerControl sender)
    {
        var writer = StartRpcImmediately(Rpc.BulkShareHostToClientSettingChange);
        CustomSetting.WriteAllHostToClientSettings(writer);
        writer.SendImmediately();
    }

    public static void RpcShareHostToClientSettingChange(this PlayerControl sender, string id, bool value)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        var writer = StartRpcImmediately(Rpc.ShareHostToClientSettingChange);
        writer.Write(id);
        writer.Write(value);
        writer.SendImmediately();
    }
    
    public static void RpcShareHostToClientSettingChange(this PlayerControl sender, string id, string value)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        var writer = StartRpcImmediately(Rpc.ShareHostToClientSettingChange);
        writer.Write(id);
        writer.Write(value);
        writer.SendImmediately();
    }
    
    public static void RpcShareHostToClientSettingChange(this PlayerControl sender, string id, float value)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        var writer = StartRpcImmediately(Rpc.ShareHostToClientSettingChange);
        writer.Write(id);
        writer.Write(value);
        writer.SendImmediately();
    }
    
    public static void RpcSetForcedTeamAssignment(this PlayerControl sender, SettingTeamPreferences teamPreference)
    {
        if (ModSettings.Local.ForcedTeamAssignment.IsLocked()) return;
        if (AmongUsClient.Instance.AmClient)
        {
            sender.SetTeamAssignmentPreference(teamPreference);
        }
        
        var writer = StartRpcImmediately(Rpc.ForcedTeamAssignment);
        writer.Write((byte)teamPreference);
        writer.SendImmediately();
    }
    
    public static void SetForcedTeamAssignment(this PlayerControl sender, SettingTeamPreferences teamPreference)
    {
        RoleManagerPatches.PlayerForcedTeams[sender.PlayerId] = teamPreference;
    }

    public static void RpcSetTeamAssignmentPreference(this PlayerControl sender, SettingTeamPreferences teamPreference)
    {
        if (AmongUsClient.Instance.AmClient)
        {
            sender.SetTeamAssignmentPreference(teamPreference);
        }
        
        var writer = StartRpcImmediately(Rpc.TeamPreference);
        writer.Write((byte)teamPreference);
        writer.SendImmediately();
    }

    public static void SetTeamAssignmentPreference(this PlayerControl sender, SettingTeamPreferences teamPreference)
    {
        RoleManagerPatches.PlayerTeamPreferences[sender.PlayerId] = teamPreference;
    }

    public static void RpcSendPrivateMessage(this PlayerControl sender, PlayerControl receiver, string message)
    {
        if (AmongUsClient.Instance.AmClient)
        {
            sender.SendPrivateMessage(receiver, message);
        }

        var writer = StartRpcImmediately(Rpc.PrivateChatMessage, receiver);
        writer.Write(receiver.OwnerId);
        writer.Write(message);
        writer.SendImmediately();
    }

    public static void SendPrivateMessage(this PlayerControl sender, PlayerControl receiver, string message)
    {
        ChatControllerPatches.AddPrivateChat(sender, receiver, message);
    }

    private static MessageWriter StartRpcImmediately(Rpc rpc, PlayerControl receiver = null, PlayerControl sender = null, bool reliable = true)
    {
        var senderId = sender ? sender.NetId : PlayerControl.LocalPlayer.NetId;
        var targetId = receiver ? receiver.OwnerId : -1;
        var writer = AmongUsClient.Instance.StartRpcImmediately(senderId, RpcId, reliable ? SendOption.Reliable : SendOption.None, targetId);
        writer.Write((uint)rpc);
        return writer;
    }

    private static void SendImmediately(this MessageWriter writer)
    {
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}