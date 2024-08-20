using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using EnoPM.BetterVanilla.Core;
using HarmonyLib;
using Hazel;
using InnerNet;
using UnityEngine;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(ChatController))]
internal static class ChatControllerPatches
{
    internal const byte PrivateMessageRpcId = 251;
    private static string _lastPrivateMessageSenderName = string.Empty;
    
    [HarmonyPrefix, HarmonyPatch(nameof(ChatController.SendFreeChat))]
    private static bool SendFreeChat(ChatController __instance)
    {
        var message = __instance.freeChatField.Text;
        if (message.StartsWith(CommandPrefix))
        {
            var command = message[1..].Split(" ").ToList();
            if (Commands.TryGetValue(command[0].ToLowerInvariant().Trim(), out var handler))
            {
                command.RemoveAt(0);
                handler(command);
                __instance.freeChatField.Clear();
            }
            else
            {
                __instance.AddChatWarning($"Unknown command: {CommandPrefix}{command[0]}");
                Plugin.Logger.LogWarning($"Unknown command: {CommandPrefix}{command[0]}");
            }

            return false;
        }
        
        ChatController.Logger.Debug($"SendFreeChat () :: Sending message: '{message}'");
        PlayerControl.LocalPlayer.RpcSendChat(message);

        return false;
    }
    
    [HarmonyPrefix, HarmonyPatch(nameof(ChatController.Awake))]
    private static void AwakePrefix()
    {
        if (!EOSManager.Instance.isKWSMinor) {
            DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
        }
    }

    internal static void HandlePrivateMessageRpc(PlayerControl sender, MessageReader reader)
    {
        var targetOwnerId = reader.ReadInt32();
        var receiver = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(x => x.OwnerId == targetOwnerId && x.Data != null);
        if (receiver == null || (receiver.OwnerId != PlayerControl.LocalPlayer.OwnerId)) return;
        _lastPrivateMessageSenderName = sender.Data.PlayerName;
        var message = reader.ReadString();
        AddPrivateChat(sender, receiver, message);
    }

    public static void AddPrivateChat(PlayerControl sender, PlayerControl receiver, string message)
    {
        HudManager.Instance.Chat.AddPrivateChat(sender, receiver, message);
    }
    
    private const string CommandPrefix = "/";
    private static readonly Dictionary<string, Func<List<string>, bool>> Commands = new()
    {
        { "kick", KickCommand },
        { "ban", BanCommand },
        { "tp", TpCommand },
        { "w", WhisperCommand },
        { "r", ReplyCommand }
    };

    private static bool ReplyCommand(List<string> arguments)
    {
        if (arguments.Count == 0 || _lastPrivateMessageSenderName == string.Empty) return false;
        return WhisperCommand($"{_lastPrivateMessageSenderName} {string.Join(" ", arguments)}".Split(" ").ToList());
    }

    private static bool WhisperCommand(List<string> arguments)
    {
        if (arguments.Count == 0) return false;
        var rawMessage = string.Join(" ", arguments);
        var target = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => rawMessage.ToLowerInvariant().StartsWith(x.Data.PlayerName.ToLowerInvariant())).MinBy(x => rawMessage[x.Data.PlayerName.Length..].Length);
        if (!target || !AmongUsClient.Instance) return false;
        var message = rawMessage[(target.Data.PlayerName.Length + 1)..];
        PlayerControl.LocalPlayer.RpcSendPrivateMessage(target, message);
        return true;
    }

    private static bool KickCommand(List<string> arguments)
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
    
    private static bool BanCommand(List<string> arguments)
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
    
    private static bool TpCommand(List<string> arguments)
    {
        if (arguments.Count == 0 || !Utils.AmDead || MeetingHud.Instance) return false;
        var playerName = string.Join(" ", arguments);
        var target = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Data.PlayerName.Equals(playerName));
        if (!target) return false;
        PlayerControl.LocalPlayer.transform.position = target.transform.position;
        return true;
    }

    private static void AddPrivateChat(this ChatController controller, PlayerControl sender, PlayerControl receiver, string chatText)
    {
        if (!sender || !receiver) return;
        var data2 = sender.Data;
        var data1 = receiver.Data;
        if (data1 == null || data2 == null) return;
        var pooledBubble = controller.GetPooledBubble();
        try
        {
            var transform = pooledBubble.transform;
            transform.SetParent(controller.scroller.Inner);
            transform.localScale = Vector3.one;
            var num = sender == PlayerControl.LocalPlayer ? 1 : 0;
            if (num != 0)
            {
                pooledBubble.SetRight();
            }
            else
            {
                pooledBubble.SetLeft();
            }

            var didVote = MeetingHud.Instance && MeetingHud.Instance.DidVote(sender.PlayerId);
            pooledBubble.SetCosmetics(data2);
            var colorId = data2.Outfits[PlayerOutfitType.Default].ColorId;
            var color = colorId < Palette.PlayerColors.Count ? Palette.PlayerColors[colorId] : Palette.PlayerColors[0];
            SetPrivateChatBubbleName(pooledBubble, data2, data1, data2.IsDead, didVote, color);
            pooledBubble.SetText(chatText);
            pooledBubble.AlignChildren();
            controller.AlignAllBubbles();
            if (!controller.IsOpenOrOpening && controller.notificationRoutine == null)
            {
                controller.notificationRoutine = controller.StartCoroutine(controller.BounceDot());
            }
            if (num != 0) return;
            SoundManager.Instance.PlaySound(controller.messageSound, false).pitch = (float) (0.5 + sender.PlayerId / 15.0);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogWarning($"ChatControllerPatches.AddPrivateChat: failed {ex}");
            controller.chatBubblePool.Reclaim(pooledBubble);
        }
    }

    private static void SetPrivateChatBubbleName(
        ChatBubble bubble,
        NetworkedPlayerInfo senderInfo,
        NetworkedPlayerInfo receiverInfo,
        bool isDead,
        bool didVote,
        Color nameColor)
    {
        var receiverName = PlayerControl.LocalPlayer.Data.PlayerName == receiverInfo.PlayerName
            ? "me"
            : receiverInfo.PlayerName;
        var playerName = $"{senderInfo.PlayerName} {Utils.Cs(Color.gray, $"[to {receiverName}]")}";
        bubble.SetName(playerName, isDead, didVote, nameColor);
    }
}