using HarmonyLib;

namespace EnoPM.BetterVanilla.Patches;

[HarmonyPatch(typeof(FreeChatInputField))]
internal static class FreeChatInputFieldPatches
{
    
    [HarmonyPostfix, HarmonyPatch(nameof(FreeChatInputField.Awake))]
    private static void AwakePostfix(FreeChatInputField __instance)
    {
        __instance.UpdateState();
        __instance.textArea.allowAllCharacters = true;
        __instance.textArea.AllowPaste = true;
        __instance.textArea.AllowSymbols = true;
        __instance.textArea.AllowEmail = true;
    }
}