using BetterVanilla.Components;
using BetterVanilla.Core.Data;
using BetterVanilla.Core.Options;
using Hazel;

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
    }

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

    public static void RpcShareHostOption(this PlayerControl sender, BaseOption option)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        var writer = sender.StartRpcImmediately(RpcIds.ShareHostOption);
        option.WriteIn(writer);
        writer.SendImmediately();
    }

    [RpcHandler(RpcIds.ShareHostOption)]
    private static void ShareHostOptionHandler(this PlayerControl sender, MessageReader reader)
    {
        var optionName = reader.ReadString();
        var option = BaseHostOption.AllOptions.Find(x => x.Name == optionName);
        if (option == null)
        {
            Ls.LogError($"Unable to find option {optionName} from RPC");
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