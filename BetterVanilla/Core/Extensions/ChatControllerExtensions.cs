using System;
using UnityEngine;

namespace BetterVanilla.Core.Extensions;

public static class ChatControllerExtensions
{
    public static void AddPrivateChat(this ChatController controller, PlayerControl sender, PlayerControl receiver, string chatText)
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
            pooledBubble.SetPrivateChatBubbleName(data2, data1, data2.IsDead, didVote, color);
            pooledBubble.SetText(chatText);
            pooledBubble.AlignChildren();
            controller.AlignAllBubbles();
            if (!controller.IsOpenOrOpening && controller.notificationRoutine == null)
            {
                controller.notificationRoutine = controller.StartCoroutine(controller.BounceDot());
            }
            if (num != 0) return;
            SoundManager.Instance.PlaySound(controller.messageSound, false).pitch = (float)(0.5 + sender.PlayerId / 15.0);
        }
        catch (Exception ex)
        {
            Ls.LogWarning($"ChatControllerPatches.AddPrivateChat: failed {ex}");
            controller.chatBubblePool.Reclaim(pooledBubble);
        }
    }
}