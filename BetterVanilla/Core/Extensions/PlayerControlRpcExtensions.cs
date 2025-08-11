using System.Linq;
using BetterVanilla.Components;
using BetterVanilla.Core.Data;
using BetterVanilla.Options;
using BetterVanilla.Options.Core.Serialization;
using Hazel;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static partial class PlayerControlRpcExtensions
{
    private enum RpcIds : uint
    {
        BetterVanillaHandshake,
        TeamPreference,
        ForcedTeamAssignment,
        PrivateChatMessage,
        ShareHostOption,
        BulkShareHostToClientSettingChange,
        SetMeetingVote,
        ShareSponsorText,
        ShareSponsorTextColor,
        ShareSponsorVisorColor
    }

    #region Handshake

    public static void RpcSendHandshake(this PlayerControl sender, BetterVanillaHandshake handshake)
    {
        if (BetterPlayerControl.LocalPlayer == null) return;
        var writer = sender.StartRpcImmediately(RpcIds.BetterVanillaHandshake);
        handshake.Serialize(writer);
        writer.SendImmediately();
    }

    [RpcHandler(RpcIds.BetterVanillaHandshake)]
    private static void SendHandshakeHandler(this PlayerControl sender, MessageReader reader)
    {
        var player = sender.gameObject.GetComponent<BetterPlayerControl>();
        if (player == null)
        {
            Ls.LogWarning($"[Rpc: {RpcIds.BetterVanillaHandshake.ToString()}] Unable to find sender's {nameof(BetterPlayerControl)}");
            return;
        }
        var handshake = new BetterVanillaHandshake(reader);
        player.SetHandshake(handshake);
    }

    #endregion

    #region Sponsors

    public static void RpcShareSponsorText(this PlayerControl sender, string sponsorText)
    {
        if (BetterPlayerControl.LocalPlayer == null) return;
        var writer = sender.StartRpcImmediately(RpcIds.ShareSponsorText);
        writer.Write(sponsorText);
        writer.SendImmediately();
    }

    [RpcHandler(RpcIds.ShareSponsorText)]
    private static void ShareSponsorTextHandler(this PlayerControl sender, MessageReader reader)
    {
        var player = sender.gameObject.GetComponent<BetterPlayerControl>();
        if (player == null)
        {
            Ls.LogWarning($"[Rpc: {RpcIds.ShareSponsorText.ToString()}] Unable to find sender's {nameof(BetterPlayerControl)}");
            return;
        }
        var sponsorText = reader.ReadString();
        player.SetSponsorText(sponsorText);
    }
    
    public static void RpcShareSponsorTextColor(this PlayerControl sender, Color color)
    {
        if (BetterPlayerControl.LocalPlayer == null) return;
        var writer = sender.StartRpcImmediately(RpcIds.ShareSponsorTextColor);
        writer.Write(color);
        writer.SendImmediately();
    }
    
    [RpcHandler(RpcIds.ShareSponsorTextColor)]
    private static void ShareSponsorTextColorHandler(this PlayerControl sender, MessageReader reader)
    {
        var player = sender.gameObject.GetComponent<BetterPlayerControl>();
        if (player == null)
        {
            Ls.LogWarning($"[Rpc: {RpcIds.ShareSponsorTextColor.ToString()}] Unable to find sender's {nameof(BetterPlayerControl)}");
            return;
        }
        var color = reader.ReadColor();
        player.SetSponsorTextColor(color);
    }
    
    public static void RpcShareSponsorVisorColor(this PlayerControl sender, Color color)
    {
        if (BetterPlayerControl.LocalPlayer == null) return;
        var writer = sender.StartRpcImmediately(RpcIds.ShareSponsorVisorColor);
        writer.Write(color);
        writer.SendImmediately();
    }
    
    [RpcHandler(RpcIds.ShareSponsorVisorColor)]
    private static void ShareSponsorVisorColorHandler(this PlayerControl sender, MessageReader reader)
    {
        var player = sender.gameObject.GetComponent<BetterPlayerControl>();
        if (player == null)
        {
            Ls.LogWarning($"[Rpc: {RpcIds.ShareSponsorVisorColor.ToString()}] Unable to find sender's {nameof(BetterPlayerControl)}");
            return;
        }
        var color = reader.ReadColor();
        player.SetVisorColor(color);
    }

    #endregion

    #region MeetingVotes

    public static void RpcSetMeetingVote(this PlayerControl sender, byte voterId, byte votedId)
    {
        if (AmongUsClient.Instance.AmClient)
        {
            MeetingHudExtensions.CastVote(voterId, votedId);
        }
        var writer = sender.StartRpcImmediately(RpcIds.SetMeetingVote);
        writer.Write(voterId);
        writer.Write(votedId);
        writer.SendImmediately();
    }

    [RpcHandler(RpcIds.SetMeetingVote)]
    private static void SetMeetingVoteHandler(this PlayerControl sender, MessageReader reader)
    {
        var voterId = reader.ReadByte();
        var votedId = reader.ReadByte();
        MeetingHudExtensions.CastVote(voterId, votedId);
    }

    #endregion

    #region TeamPreference
    
    public static void RpcSetForcedTeamAssignment(this PlayerControl sender, TeamPreferences teamPreference)
    {
        if (AmongUsClient.Instance.AmClient)
        {
            sender.SetForcedTeamAssignment(teamPreference);
        }
        var writer = sender.StartRpcImmediately(RpcIds.ForcedTeamAssignment);
        writer.Write((uint)teamPreference);
        writer.SendImmediately();
    }
    
    [RpcHandler(RpcIds.ForcedTeamAssignment)]
    private static void SetForcedTeamAssignmentHandler(this PlayerControl sender, MessageReader reader)
    {
        var preference = (TeamPreferences)reader.ReadUInt32();
        sender.SetForcedTeamAssignment(preference);
    }
    
    private static void SetForcedTeamAssignment(this PlayerControl sender, TeamPreferences teamPreference)
    {
        BetterVanillaManager.Instance.AllForcedTeamAssignments[sender.OwnerId] = teamPreference;
    }

    public static void RpcSetTeamPreference(this PlayerControl sender, TeamPreferences teamPreference)
    {
        if (AmongUsClient.Instance.AmClient)
        {
            sender.SetTeamPreference(teamPreference);
        }
        var writer = sender.StartRpcImmediately(RpcIds.TeamPreference);
        writer.Write((uint)teamPreference);
        writer.SendImmediately();
    }

    [RpcHandler(RpcIds.TeamPreference)]
    private static void SetTeamPreferenceHandler(this PlayerControl sender, MessageReader reader)
    {
        sender.SetTeamPreference((TeamPreferences)reader.ReadUInt32());
    }
    
    private static void SetTeamPreference(this PlayerControl sender, TeamPreferences teamPreference)
    {
        BetterVanillaManager.Instance.AllTeamPreferences[sender.OwnerId] = teamPreference;
    }

    #endregion

    #region ShareHostOption
    public static void RpcShareHostOption(this PlayerControl sender, AbstractSerializableOption option)
    {
        if (!LocalConditions.AmHost()) return;
        var writer = sender.StartRpcImmediately(RpcIds.ShareHostOption);
        writer.Write(option.Key);
        option.WriteValue(writer);
        writer.SendImmediately();
    }

    [RpcHandler(RpcIds.ShareHostOption)]
    private static void ShareHostOptionHandler(this PlayerControl sender, MessageReader reader)
    {
        var optionKey = reader.ReadString();
        var option = HostOptions.Default.GetOptions().FirstOrDefault(x => x.Key == optionKey);
        if (option == null)
        {
            Ls.LogWarning($"Unable to find option {optionKey} from Rpc");
            return;
        }
        option.ReadValue(reader);
    }

    #endregion

    #region PrivateChatMessage

    public static void RpcSendPrivateMessage(this PlayerControl sender, PlayerControl receiver, string message)
    {
        if (AmongUsClient.Instance.AmClient)
        {
            sender.SendPrivateMessage(receiver, message);
        }

        var writer = sender.StartRpcImmediately(RpcIds.PrivateChatMessage, receiver);
        writer.Write(receiver.OwnerId);
        writer.Write(message);
        writer.SendImmediately();
    }

    [RpcHandler(RpcIds.PrivateChatMessage)]
    private static void SendPrivateMessageHandler(this PlayerControl sender, MessageReader reader)
    {
        ChatCommandsManager.HandlePrivateMessageRpc(sender, reader);
    }

    private static void SendPrivateMessage(this PlayerControl sender, PlayerControl receiver, string message)
    {
        if (!HudManager.Instance || !HudManager.Instance.Chat)
        {
            Ls.LogError($"{nameof(SendPrivateMessage)} failed: HudManager not instantiated");
            return;
        }
        HudManager.Instance.Chat.AddPrivateChat(sender, receiver, message);
    }

    #endregion
}