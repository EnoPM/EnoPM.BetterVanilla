using System;
using System.Collections.Generic;
using System.Linq;
using BetterVanilla.Core.Extensions;
using BetterVanilla.Core.Helpers;
using Hazel;

namespace BetterVanilla.Core;

public sealed class ChatCommandsManager
{
    private const string CommandPrefix = "/";
    private static string LastPrivateMessageSenderName { get; set; }
    
    private readonly Dictionary<string, Func<List<string>, bool>> _commands = new()
    {
        { "kick", KickCommandHandler },
        { "ban", BanCommandHandler },
        { "tp", TpCommandHandler },
        { "w", WhisperCommandHandler },
        { "r", ReplyCommandHandler },
    };

    public bool IsChatCommand(string message)
    {
        return message.StartsWith(CommandPrefix);
    }

    public bool ExecuteCommand(string message)
    {
        var command = message[CommandPrefix.Length..].Split(" ").ToList();
        if (_commands.TryGetValue(command[0].ToLowerInvariant().Trim(), out var commandHandler))
        {
            command.RemoveAt(0);
            commandHandler(command);
            return true;
        }
        return false;
    }

    public static void HandlePrivateMessageRpc(PlayerControl sender, MessageReader reader)
    {
        var targetOwnerId = reader.ReadInt32();
        var receiver = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(x => x.OwnerId == targetOwnerId && x.Data != null);
        if (receiver == null || (receiver.OwnerId != PlayerControl.LocalPlayer.OwnerId)) return;
        LastPrivateMessageSenderName = sender.Data.PlayerName;
        var message = reader.ReadString();
        HudManager.Instance.Chat.AddPrivateChat(sender, receiver, message);
    }
    
    private static bool KickCommandHandler(List<string> arguments)
    {
        if (arguments.Count == 0) return false;
        var playerName = string.Join(" ", arguments);
        var target = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
        if (!target || !AmongUsClient.Instance || !AmongUsClient.Instance.CanBan()) return false;
        var client = AmongUsClient.Instance.GetClient(target.OwnerId);
        if (client == null) return false; 
        AmongUsClient.Instance.KickPlayer(client.Id, false);
        return true;
    }
    
    private static bool BanCommandHandler(List<string> arguments)
    {
        if (arguments.Count == 0) return false;
        var playerName = string.Join(" ", arguments);
        var target = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
        if (target == null || AmongUsClient.Instance == null || !AmongUsClient.Instance.CanBan()) return false;
        var client = AmongUsClient.Instance.GetClient(target.OwnerId);
        if (client == null) return false; 
        AmongUsClient.Instance.KickPlayer(client.Id, true);
        return true;
    }
    
    private static bool TpCommandHandler(List<string> arguments)
    {
        if (arguments.Count == 0 || !LocalConditions.AmDead() || MeetingHud.Instance) return false;
        var playerName = string.Join(" ", arguments);
        var target = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
        if (!target) return false;
        PlayerControl.LocalPlayer.transform.position = target.transform.position;
        return true;
    }

    private static bool WhisperCommandHandler(List<string> arguments)
    {
        if (arguments.Count == 0) return false;
        var rawMessage = string.Join(" ", arguments);
        var target = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => rawMessage.ToLowerInvariant().StartsWith(x.Data.PlayerName.ToLowerInvariant()))
            .MinBy(x => rawMessage[x.Data.PlayerName.Length..].Length);
        if (!target || !AmongUsClient.Instance) return false;
        var message = rawMessage[(target.Data.PlayerName.Length + 1)..];
        PlayerControl.LocalPlayer.RpcSendPrivateMessage(target, message);
        return true;
    }
    
    private static bool ReplyCommandHandler(List<string> arguments)
    {
        if (arguments.Count == 0 || LastPrivateMessageSenderName == string.Empty) return false;
        return WhisperCommandHandler($"{LastPrivateMessageSenderName} {string.Join(" ", arguments)}".Split(" ").ToList());
    }
}