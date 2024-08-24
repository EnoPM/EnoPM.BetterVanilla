using BetterVanilla.Components;
using HarmonyLib;

namespace BetterVanilla.Core.Patches;

[HarmonyPatch(typeof(ChatController))]
internal static class ChatControllerPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(ChatController.SendFreeChat))]
    private static bool SendFreeChatPrefix(ChatController __instance)
    {
        var message = __instance.freeChatField.Text;
        if (BetterVanillaManager.Instance.ChatCommands.IsChatCommand(message))
        {
            if (BetterVanillaManager.Instance.ChatCommands.ExecuteCommand(message))
            {
                __instance.freeChatField.Clear();
            }
            else
            {
                __instance.AddChatWarning($"Unknown command: {message}");
            }

            return false;
        }
        
        ChatController.Logger.Debug($"SendFreeChat () :: Sending message: '{message}'");
        PlayerControl.LocalPlayer.RpcSendChat(message);

        return false;
    }
}